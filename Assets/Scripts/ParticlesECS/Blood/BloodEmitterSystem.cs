using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Rendering;
using Unity.Jobs;
using Unity.Burst;

//TODO change to jobs
public class BloodEmitterSystem : ComponentSystem
{
    private ComponentType tag;

    private int numPerUpdate;
    private float rate;
    private bool initVals;
    private float dt;
    private Random rand;
    private float timer = 0;
    private Entity emitter;
    protected override void OnCreate()
    {
        //=================== just change this to reuse this system for other types of particles
        tag = ComponentType.ReadOnly<BloodTag>();
        //======================================================================================

        initVals = false;
        dt = 0;
        rand.InitState();
        timer = 99999;
    }
    protected override void OnUpdate()
    {        
        if (!initVals)
        { //init
            EntityQuery q = GetEntityQuery(tag, ComponentType.ReadOnly<ParticleSystemData>());
            NativeArray<Entity> ent = q.ToEntityArray(Allocator.TempJob);
            NativeArray<ParticleSystemData> temp = q.ToComponentDataArray<ParticleSystemData>(Allocator.TempJob);
            if (temp.Length > 0)
            {
                initVals = true;
                numPerUpdate = temp[0].numPerUpdate;
                rate = temp[0].rate;
                for (int i = 1; i < ent.Length; ++i) //destory extra ones
                    PostUpdateCommands.DestroyEntity(ent[i]);
                emitter = ent[0];
            } 
            temp.Dispose();
            ent.Dispose();
        }

        dt = Time.DeltaTime;
        timer += dt;
        var emitTag = GetComponentDataFromEntity<ParticleEmitTag>(true);
        if (emitTag.HasComponent(emitter) && timer >= rate)
        {
            timer = 0;
            //get disabled particles and make it alive        
            EntityQuery q = GetEntityQuery(
                tag,
                ComponentType.ReadOnly<Disabled>(),
                typeof(Translation),
                typeof(Rotation),
                typeof(Scale),
                typeof(ParticleEntityData)
                );

            NativeArray<Entity> arr = q.ToEntityArray(Allocator.TempJob);
            NativeArray<Translation> trans = q.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Rotation> rots = q.ToComponentDataArray<Rotation>(Allocator.TempJob);
            NativeArray<Scale> scales = q.ToComponentDataArray<Scale>(Allocator.TempJob);
            NativeArray<ParticleEntityData> dataArr = q.ToComponentDataArray<ParticleEntityData>(Allocator.TempJob);

            for (int i = 0; i < numPerUpdate && i < arr.Length; ++i)
            {
                Entity e = arr[i];
                Translation t = trans[i];
                Rotation r = rots[i];
                Scale s = scales[i];
                ParticleEntityData datum = dataArr[i];

                datum.rot = datum.startRot;
                datum.vel = datum.startVel;

                t.Value = rand.NextFloat3(datum.emitAreaMin, datum.emitAreaMax);
                r.Value = quaternion.Euler(datum.rot);
                s.Value = datum.startScale;

                datum.life = datum.lifeTime;           
                
                PostUpdateCommands.SetComponent(e, t);
                PostUpdateCommands.SetComponent(e, r);
                PostUpdateCommands.SetComponent(e, s);
                PostUpdateCommands.SetComponent(e, datum);
                PostUpdateCommands.AddComponent(e, typeof(ParticleAliveTag));
                PostUpdateCommands.RemoveComponent(arr[i], typeof(Disabled));
            }
            arr.Dispose();
            trans.Dispose();
            rots.Dispose();
            scales.Dispose();
            dataArr.Dispose();
        }

        //update alive particles
        Entities.WithAllReadOnly(tag, typeof(ParticleAliveTag)).
            ForEach((Entity e, ref Translation t, ref Rotation r, ref Scale s, ref ParticleEntityData data) => {
                float percentLife = 1 - (data.life / data.lifeTime);

                data.rot = math.lerp(data.startRot, data.endRot, percentLife);
                data.vel = math.lerp(data.startVel, data.endVel, percentLife);

                t.Value += data.vel * dt;
                r.Value = quaternion.Euler(data.rot);
                s.Value = math.lerp(data.startScale, data.endScale, percentLife);

                data.life -= dt;


                if (data.life <= 0)
                {
                    PostUpdateCommands.RemoveComponent(e, typeof(ParticleAliveTag));
                    PostUpdateCommands.AddComponent(e, typeof(Disabled));
                }
            });
    }
}