using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
[UpdateBefore(typeof(BloodEmitterJobSystem))]
public class BloodEmitterQueryJobSystem : JobComponentSystem
{
    [ReadOnly] private ComponentDataFromEntity<ParticleEmitTag> q;
    private BloodEmitterJobSystem initSystem;
    private NativeArray<bool> enabled;
    private NativeArray<int> numPerUpdate;
    private NativeArray<float> rate;
    private NativeArray<int> numJobBatch;

    protected override void OnCreate()
    {
        initSystem = World.GetOrCreateSystem<BloodEmitterJobSystem>();
    }

    private struct SearchJob : IJobForEachWithEntity<BloodTag, ParticleSystemData>
    {
        public NativeArray<bool> enabled;
        public NativeArray<int> numPerUpdate;
        public NativeArray<float> rate;
        public NativeArray<int> numJobBatch;

        [ReadOnly] public ComponentDataFromEntity<ParticleEmitTag> query;
        public void Execute(Entity entity, int index, [ReadOnly] ref BloodTag tag, [ReadOnly] ref ParticleSystemData data)
        {
            if (query.HasComponent(entity))
                enabled[0] = true;
            numPerUpdate[0] = data.numPerUpdate;
            rate[0] = data.rate;
            numJobBatch[0] = data.numJobBatch;
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        q = GetComponentDataFromEntity<ParticleEmitTag>(true);
        enabled = new NativeArray<bool>(1, Allocator.TempJob);
        numPerUpdate = new NativeArray<int>(1, Allocator.TempJob);
        rate = new NativeArray<float>(1, Allocator.TempJob);
        numJobBatch = new NativeArray<int>(1, Allocator.TempJob);

        SearchJob job = new SearchJob
        {
            enabled = enabled,
            numPerUpdate = numPerUpdate,
            rate = rate,
            numJobBatch = numJobBatch,
            query = q,
        };
        var handle = job.Schedule(this, inputDeps);
        handle.Complete();
        initSystem.enabled = job.enabled[0];
        initSystem.numPerUpdate = job.numPerUpdate[0];
        initSystem.rate = job.rate[0];
        initSystem.numJobBatch = job.numJobBatch[0];
        enabled.Dispose();
        numPerUpdate.Dispose();
        rate.Dispose();
        numJobBatch.Dispose();
        return handle;
    }
}

//TODO change to jobs
//[UpdateAfter(typeof(BloodEmitterQueryJobSystem))]
//public class BloodEmitterSystem : ComponentSystem
//{
//    private ComponentType tag;

//    public bool enabled;
//    public int numPerUpdate;
//    public float rate;

//    private Random rand;
//    private float timer;
//    protected override void OnCreate()
//    {
//        //=================== just change this to reuse this system for other types of particles
//        tag = ComponentType.ReadOnly<BloodTag>();
//        //======================================================================================

//        numPerUpdate = 1;
//        rate = 0.1f;

//        rand.InitState();
//        timer = 0;
//    }
//    protected override void OnUpdate()
//    {
//        timer += Time.DeltaTime;
//        if (enabled && timer >= rate)
//        {
//            timer = 0;
//            //get disabled particles and make it alive        
//            EntityQuery q = GetEntityQuery(
//                tag,
//                ComponentType.ReadOnly<Disabled>(),
//                typeof(Translation),
//                typeof(Rotation),
//                typeof(Scale),
//                typeof(ParticleEntityData)
//                );

//            NativeArray<Entity> arr = q.ToEntityArray(Allocator.TempJob);
//            NativeArray<Translation> trans = q.ToComponentDataArray<Translation>(Allocator.TempJob);
//            NativeArray<Rotation> rots = q.ToComponentDataArray<Rotation>(Allocator.TempJob);
//            NativeArray<Scale> scales = q.ToComponentDataArray<Scale>(Allocator.TempJob);
//            NativeArray<ParticleEntityData> dataArr = q.ToComponentDataArray<ParticleEntityData>(Allocator.TempJob);

//            for (int i = 0; i < numPerUpdate && i < arr.Length; ++i)
//            {
//                Entity e = arr[i];
//                Translation t = trans[i];
//                Rotation r = rots[i];
//                Scale s = scales[i];
//                ParticleEntityData datum = dataArr[i];

//                datum.rot = datum.startRot;
//                datum.vel = datum.startVel;

//                t.Value = rand.NextFloat3(datum.emitAreaMin, datum.emitAreaMax);
//                r.Value = quaternion.Euler(datum.rot);
//                s.Value = datum.startScale;

//                datum.life = datum.lifeTime;

//                PostUpdateCommands.SetComponent(e, t);
//                PostUpdateCommands.SetComponent(e, r);
//                PostUpdateCommands.SetComponent(e, s);
//                PostUpdateCommands.SetComponent(e, datum);
//                PostUpdateCommands.AddComponent(e, typeof(ParticleAliveTag));
//                PostUpdateCommands.RemoveComponent(arr[i], typeof(Disabled));
//            }
//            arr.Dispose();
//            trans.Dispose();
//            rots.Dispose();
//            scales.Dispose();
//            dataArr.Dispose();
//        }
//    }
//}

[BurstCompile]
[UpdateAfter(typeof(BloodEmitterQueryJobSystem))]
public class BloodEmitterJobSystem : JobComponentSystem
{
    public bool enabled;
    public int numPerUpdate;
    public float rate;
    public int numJobBatch;

    private EndSimulationEntityCommandBufferSystem ecbs;
    private float timer;

    protected override void OnCreate()
    {
        ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        timer = 0;
    }

    private struct Particle
    {
        public Entity e;
        public Translation t;
        public Rotation r;
        public Scale s;
        public ParticleEntityData data;
    };

    private struct GetParticlesJob : IJobParallelFor
    {
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Entity> ents;
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Translation> trans;
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Rotation> rots;
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Scale> scales;
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<ParticleEntityData> dataArr;
        public NativeArray<Particle> particles;
        public void Execute(int index)
        {
            Particle p = new Particle();
            p.e = ents[index];
            p.t = trans[index];
            p.r = rots[index];
            p.s = scales[index];
            p.data = dataArr[index];
            particles[index] = p;
        }
    }

    private struct EmitJob : IJobParallelFor
    {
        [ReadOnly] [DeallocateOnJobCompletion] public NativeArray<Particle> particles;
        [ReadOnly] public float dt;
        public EntityCommandBuffer.Concurrent ecb;
        public Random rand;

        public void Execute(int index)
        {
            Entity e = particles[index].e;
            Translation t = particles[index].t;
            Rotation r = particles[index].r;
            Scale s = particles[index].s;
            ParticleEntityData data = particles[index].data;

            data.startRot = data.startRotMin;
            data.startScale = data.startScaleMin;
            data.startVel = data.startVelMin;
            if (data.rotOverTime)
                data.endRot = data.endRotMin;
            if (data.scaleOverTime)
                data.endScale = data.endScaleMin;
            if (data.velOverTime)            
                data.endVel = data.endVelMin;

            data.rot = data.startRot;
            data.vel = data.startVel;
            data.gravity = data.startGravity;

            t.Value = rand.NextFloat3(data.emitAreaMin, data.emitAreaMax);
            r.Value = quaternion.Euler(data.rot);
            s.Value = data.startScaleMin;

            data.life = data.lifeTime;

            ecb.SetComponent(index, particles[index].e, t);
            ecb.SetComponent(index, particles[index].e, r);
            ecb.SetComponent(index, particles[index].e, s);
            ecb.SetComponent(index, particles[index].e, data);
            ecb.SetComponent(index, particles[index].e, data);
            ecb.AddComponent(index, particles[index].e, typeof(ParticleAliveTag));
            ecb.RemoveComponent(index, particles[index].e, typeof(Disabled));
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (!enabled)
            return new JobHandle();
        timer += Time.DeltaTime;
        if (timer < rate)
            return new JobHandle();

        EntityQuery q = GetEntityQuery(            
            ComponentType.ReadOnly<BloodTag>(),
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

        int count = math.min(arr.Length, numPerUpdate);

        NativeArray<Particle> particles = new NativeArray<Particle>(count, Allocator.TempJob);
        GetParticlesJob getJob = new GetParticlesJob
        { 
            ents = arr,
            trans = trans,
            rots = rots,
            scales = scales,
            dataArr = dataArr,
            particles = particles,
        };
        var handle = getJob.Schedule(count, numJobBatch, inputDeps);
        handle.Complete();
        
        EmitJob emitJob = new EmitJob
        {
            particles = particles,
            dt = Time.DeltaTime,
            ecb = ecbs.CreateCommandBuffer().ToConcurrent(),
            rand = new Random((uint)UnityEngine.Random.Range(1, 100000)),
        };
        handle = emitJob.Schedule(count, numJobBatch, inputDeps);
        ecbs.AddJobHandleForProducer(handle);
        return handle;

    }
}

[BurstCompile]
[UpdateAfter(typeof(BloodEmitterJobSystem))]
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

            if (data.rotOverTime)
                data.rot = math.lerp(data.startRot, data.endRot, percentLife);
            if (data.velOverTime)
                data.vel = math.lerp(data.startVel, data.endVel, percentLife);
            if (data.gravityOverTime)
                data.gravity = math.lerp(data.startGravity, data.endGravity, percentLife);

            data.vel += data.gravity;

            t.Value += data.vel * dt;
            r.Value = quaternion.Euler(data.rot);
            if (data.scaleOverTime)
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