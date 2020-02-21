using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
[UpdateBefore(typeof(BloodEmitterSystem))]
public class BloodEmitterQueryJobSystem : JobComponentSystem
{
    [ReadOnly] private ComponentDataFromEntity<ParticleEmitTag> q;
    private BloodEmitterSystem initSystem;
    private NativeArray<bool> enabled;
    private NativeArray<int> numPerUpdate;
    private NativeArray<float> rate;

    protected override void OnCreate()
    {
        initSystem = World.GetOrCreateSystem<BloodEmitterSystem>();
        enabled = new NativeArray<bool>(1, Allocator.TempJob);
        numPerUpdate = new NativeArray<int>(1, Allocator.TempJob);
        rate = new NativeArray<float>(1, Allocator.TempJob);
    }

    protected override void OnDestroy()
    {
        enabled.Dispose();
        numPerUpdate.Dispose();
        rate.Dispose();
    }
    private struct SearchJob : IJobForEachWithEntity<BloodTag, ParticleSystemData>
    {
        public NativeArray<bool> enabled;
        public NativeArray<int> numPerUpdate;
        public NativeArray<float> rate;
        [ReadOnly] public ComponentDataFromEntity<ParticleEmitTag> query;
        public void Execute(Entity entity, int index, [ReadOnly] ref BloodTag tag, [ReadOnly] ref ParticleSystemData data)
        {
            if (query.HasComponent(entity))
                enabled[0] = true;
            numPerUpdate[0] = data.numPerUpdate;
            rate[0] = data.rate;
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        q = GetComponentDataFromEntity<ParticleEmitTag>(true);

        SearchJob job = new SearchJob
        {
            enabled = enabled,
            numPerUpdate = numPerUpdate,
            rate = rate,
            query = q,
    };
        var handle = job.Schedule(this, inputDeps);
        handle.Complete();
        initSystem.enabled = job.enabled[0];
        initSystem.numPerUpdate = job.numPerUpdate[0];
        initSystem.rate = job.rate[0];
        return handle;
    }
}

//TODO change to jobs
[UpdateAfter(typeof(BloodEmitterQueryJobSystem))]
public class BloodEmitterSystem : ComponentSystem
{
    private ComponentType tag;

    public bool enabled;
    public int numPerUpdate;
    public float rate;

    private Random rand;
    private float timer;
    protected override void OnCreate()
    {
        //=================== just change this to reuse this system for other types of particles
        tag = ComponentType.ReadOnly<BloodTag>();
        //======================================================================================

        numPerUpdate = 1;
        rate = 0.1f;

        rand.InitState();
        timer = 0;
    }
    protected override void OnUpdate()
    {
        timer += Time.DeltaTime;
        if (enabled && timer >= rate)
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
    }
}

//[BurstCompile]
//[UpdateAfter(typeof(BloodEmitterQueryJobSystem))]
//public class BloodEmitterJobSystem : JobComponentSystem
//{
//    public bool enabled;
//    public int numPerUpdate;
//    public float rate;

//    private EndSimulationEntityCommandBufferSystem ecbs;
//    private float timer;
//    private Random rand;
//    protected override void OnCreate()
//    {
//        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//        timer = 0;
//        rand.InitState();
//    }

//    private struct EmitJob : IJobForEachWithEntity<BloodTag, Disabled, Translation, Rotation, Scale, ParticleEntityData>
//    {
//        [ReadOnly] public float dt;
//        public EntityCommandBuffer.Concurrent ecb;

//        public void Execute(Entity entity, int index, [ReadOnly] ref BloodTag tag1, [ReadOnly] ref Disabled tag2, ref Translation t, ref Rotation r, ref Scale s, ref ParticleEntityData data)
//        {
//        }
//    }

//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        if (!enabled)
//            return new JobHandle();
//        timer += Time.DeltaTime;
//        if (timer > rate)
//        {
//            EmitJob job = new EmitJob
//            {
//                dt = Time.DeltaTime,
//                ecb = ecbs.CreateCommandBuffer().ToConcurrent(),
//            };
//            var handle = job.Schedule(this, inputDeps);
//            ecbs.AddJobHandleForProducer(handle);
//            return handle;
//        }        
//        return new JobHandle();
//    }
//}

[BurstCompile]
[UpdateAfter(typeof(BloodEmitterSystem))]
public class BloodEmitterMoveJobSystem : JobComponentSystem
{
    private EndSimulationEntityCommandBufferSystem ecbs;
    protected override void OnCreate()
    {
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    private struct MoveJob : IJobForEachWithEntity<BloodTag, ParticleAliveTag, Translation, Rotation, Scale, ParticleEntityData>
    {
        [ReadOnly] public float dt;
        public EntityCommandBuffer.Concurrent ecb;
        public void Execute(Entity entity, int index, [ReadOnly] ref BloodTag tag1, [ReadOnly] ref ParticleAliveTag tag2, ref Translation t, ref Rotation r, ref Scale s, ref ParticleEntityData data)
        {
            float percentLife = 1 - (data.life / data.lifeTime);

            data.rot = math.lerp(data.startRot, data.endRot, percentLife);
            data.vel = math.lerp(data.startVel, data.endVel, percentLife);

            t.Value += data.vel * dt;
            r.Value = quaternion.Euler(data.rot);
            s.Value = math.lerp(data.startScale, data.endScale, percentLife);

            data.life -= dt;


            if (data.life <= 0)
            {
                ecb.RemoveComponent(index, entity, typeof(ParticleAliveTag));
                ecb.AddComponent(index, entity, typeof(Disabled));
            }
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        MoveJob job = new MoveJob
        {
            dt = Time.DeltaTime,
            ecb = ecbs.CreateCommandBuffer().ToConcurrent(),
        };
        var handle = job.Schedule(this, inputDeps);
        ecbs.AddJobHandleForProducer(handle);
        return handle;
    }
}