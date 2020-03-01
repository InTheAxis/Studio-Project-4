namespace ECSParticles
{
    using Unity.Entities;
    using Unity.Transforms;
    using Unity.Mathematics;
    using Unity.Collections;
    using Unity.Jobs;
    using Unity.Burst;
    using Unity.Rendering;

    namespace Blood
    {
        using MY_TAG = BloodTag;


        [BurstCompile]
        [UpdateBefore(typeof(EmitterJobSystem))]
        public class EmitterQueryJobSystem : JobComponentSystem
        {
            public bool enabled { private set; get; }
            public ParticleSystemData data { private set; get; }

            [ReadOnly] private ComponentDataFromEntity<ParticleEmitTag> q;
            private NativeArray<bool> enabledResult;
            private NativeArray<ParticleSystemData> dataResult;
            
            private EndSimulationEntityCommandBufferSystem ecbs;            
            private bool emissionDone;
            public void EmissionDone() { emissionDone = true; }

            protected override void OnCreate()
            {
                emissionDone = false;
                enabled = false;
                ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            }

            private struct SearchJob : IJobForEachWithEntity<MY_TAG, ParticleSystemData>
            {
                public NativeArray<bool> enabled;
                public NativeArray<ParticleSystemData> data;
                public EntityCommandBuffer.Concurrent ecb;
                [ReadOnly] public bool emissionDone;

                [ReadOnly] public ComponentDataFromEntity<ParticleEmitTag> query;
                public void Execute(Entity entity, int index, [ReadOnly] ref MY_TAG tag, [ReadOnly] ref ParticleSystemData _data)
                {
                    if (query.HasComponent(entity))
                    {
                        if (emissionDone)
                        {
                            enabled[0] = false;
                            ecb.RemoveComponent(index, entity, typeof(ParticleEmitTag));
                        }
                        else
                            enabled[0] = true;
                    }
                    else
                        enabled[0] = false;

                    data[0] = _data;
                }
            }
            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                q = GetComponentDataFromEntity<ParticleEmitTag>(true);
                enabledResult = new NativeArray<bool>(1, Allocator.TempJob);
                dataResult = new NativeArray<ParticleSystemData>(1, Allocator.TempJob);

                SearchJob job = new SearchJob
                {
                    enabled = enabledResult,
                    data = dataResult,
                    query = q,
                    ecb = ecbs.CreateCommandBuffer().ToConcurrent(),
                    emissionDone = emissionDone,
                };
                var handle = job.Schedule(this, inputDeps);
                handle.Complete();
               
                enabled = job.enabled[0];
                data = job.data[0];
                enabledResult.Dispose();
                dataResult.Dispose();

                if (!enabled)
                    emissionDone = false;

                return handle;
            }
        }

        [BurstCompile]
        [UpdateAfter(typeof(EmitterQueryJobSystem))]
        public class EmitterJobSystem : JobComponentSystem
        {
            private bool enabled;
            private ParticleSystemData sysData;

            private EmitterQueryJobSystem queryJobSystem;
            private EndSimulationEntityCommandBufferSystem ecbs;
            private float timer;
            private float elapsed;

            protected override void OnCreate()
            {
                queryJobSystem = World.GetOrCreateSystem<EmitterQueryJobSystem>();
                ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
                timer = 0;
                elapsed = 0;
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
                [ReadOnly] public float3 pos;
                [ReadOnly] public float3 dir;
                [ReadOnly] public float3 posRandom;
                [ReadOnly] public int emitDirType;
                public EntityCommandBuffer.Concurrent ecb;
                public Random rand;
                public void Execute(int index)
                {
                    Entity e = particles[index].e;
                    Translation t = particles[index].t;
                    Rotation r = particles[index].r;
                    Scale s = particles[index].s;
                    ParticleEntityData data = particles[index].data;

                    data._startRot = data.startRot + rand.NextFloat3(-data.rotRandom, data.rotRandom);
                    data._startScale = data.startScale + rand.NextFloat(-data.scaleRandom, data.scaleRandom);
                    data._startGravity = data.startGravity + rand.NextFloat3(-data.gravityRandom, data.gravityRandom);

                    float3 emitDir = dir;
                    switch (emitDirType)
                    {
                        case 0:
                        default:
                            //emitDir = dir;
                            break;
                        case 1:
                            emitDir = rand.NextFloat3(new float3(-1, -1, -1), new float3(1, 1, 1));
                            break;
                    }
                    data._startVel = data.startSpeed * emitDir + rand.NextFloat3(-data.velRandom, data.velRandom);

                    data._endRot = data.endRot + rand.NextFloat3(-data.rotRandom, data.rotRandom);
                    data._endScale = data.endScale + rand.NextFloat(-data.scaleRandom, data.scaleRandom);
                    data._endVel = data.endVel;
                    data._endGravity = data.endGravity + rand.NextFloat3(-data.gravityRandom, data.gravityRandom);

                    //set internal vals
                    data.emitSource = pos;
                    data.emitDir = emitDir;
                    data.rot = data._startRot;
                    data.vel = data._startVel;
                    data.gravity = data._startGravity;
                    data.life = data.lifeTime + rand.NextFloat(-data.lifeTimeRandom, data.lifeTimeRandom);
                    
                    t.Value = data.emitSource + rand.NextFloat3(-posRandom, posRandom);
                    r.Value = quaternion.Euler(data.rot);
                    s.Value = data._startScale;

                    ecb.SetComponent(index, particles[index].e, t);
                    ecb.SetComponent(index, particles[index].e, r);
                    ecb.SetComponent(index, particles[index].e, s);
                    ecb.SetComponent(index, particles[index].e, data);
                    ecb.AddComponent(index, particles[index].e, typeof(ParticleAliveTag));
                    ecb.RemoveComponent(index, particles[index].e, typeof(FrozenRenderSceneTag)); //render entity
                }
            }

            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                enabled = queryJobSystem.enabled;
                sysData = queryJobSystem.data;

                if (!enabled)
                {
                    elapsed = 0;
                    return new JobHandle();
                }

                elapsed += Time.DeltaTime;
                if (elapsed < sysData.delay)
                    return new JobHandle();

                if (!sysData.loop && elapsed - sysData.delay >= sysData.duration)
                {
                    //tell query to stop
                    queryJobSystem.EmissionDone();
                    return new JobHandle();
                }

                timer += Time.DeltaTime;
                if (timer < sysData.rate)
                    return new JobHandle();

                timer = 0;

                EntityQuery q = GetEntityQuery(
                    ComponentType.ReadOnly<MY_TAG>(),
                    ComponentType.ReadOnly<FrozenRenderSceneTag>(), //only need to care about those that are not being rendered
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
                    
                int count = math.min(arr.Length, sysData.numPerUpdate);

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
                var getHandle = getJob.Schedule(count, sysData.numJobBatch, inputDeps);
                getHandle.Complete();

                EmitJob emitJob = new EmitJob
                {
                    particles = particles,
                    pos = sysData.pos,
                    dir = sysData.dir,
                    posRandom = sysData.emitPosRandom,
                    emitDirType = sysData.emitDirType,
                    ecb = ecbs.CreateCommandBuffer().ToConcurrent(),
                    rand = new Random((uint)UnityEngine.Random.Range(1, 100000)),
                };
                var emitHandle = emitJob.Schedule(count, sysData.numJobBatch, getHandle);
                ecbs.AddJobHandleForProducer(emitHandle);
                emitHandle.Complete();
                return emitHandle;

            }
        }

        [BurstCompile]
        [UpdateAfter(typeof(EmitterJobSystem))]
        public class EmitterMoveJobSystem : JobComponentSystem
        {
            private EndSimulationEntityCommandBufferSystem ecbs;
            protected override void OnCreate()
            {
                ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            }
            private struct MoveJob : IJobForEachWithEntity<MY_TAG, ParticleAliveTag, Translation, Rotation, Scale, ParticleEntityData>
            {
                [ReadOnly] public float dt;
                public EntityCommandBuffer.Concurrent ecb;
                public void Execute(Entity entity, int index, [ReadOnly] ref MY_TAG tag1, [ReadOnly] ref ParticleAliveTag tag2, ref Translation t, ref Rotation r, ref Scale s, ref ParticleEntityData data)
                {
                    data.life -= dt;

                    if (data.life <= 0)
                    {
                        ecb.RemoveComponent(index, entity, typeof(ParticleAliveTag));
                        ecb.AddComponent(index, entity, typeof(FrozenRenderSceneTag)); //dont render entity
                        t.Value.y = -99999; //in case its still on screen
                    }

                    float percentLife = 1 - (data.life / data.lifeTime);

                    if (data.rotOverTime)
                        data.rot = math.lerp(data._startRot, data._endRot, percentLife);
                    r.Value = quaternion.Euler(data.rot);
                    if (data.scaleOverTime)
                        s.Value = math.lerp(data._startScale, data._endScale, percentLife);
                    if (data.velOverTime)
                        data.vel = math.lerp(data._startVel, data._endVel, percentLife);
                    if (data.gravityOverTime)
                        data.gravity = math.lerp(data._startGravity, data._endGravity, percentLife);

                    if (data.gravTowardsSource)
                    {
                        float3 dir = t.Value - data.emitSource;
                        if (math.length(dir) > 0.1)
                            data.vel += math.length(data.gravity) * -math.normalize(dir) * dt;
                    }
                    else
                        data.vel += data.gravity * dt;
                    t.Value += data.vel * dt;
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
                handle.Complete();
                return handle;
            }
        }


        [BurstCompile]
        [DisableAutoCreation]
        public class EmitterCleanUpJobSystem : JobComponentSystem
        {
            private EndSimulationEntityCommandBufferSystem ecbs;

            protected override void OnCreate()
            {
                ecbs = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            }

            private struct CleanUpJob : IJobForEachWithEntity<MY_TAG>
            {
                public EntityCommandBuffer.Concurrent ecb;
                public void Execute(Entity entity, int index, [ReadOnly] ref MY_TAG c0)
                {
                    ecb.DestroyEntity(index, entity);
                }
            }
            protected override JobHandle OnUpdate(JobHandle inputDeps)
            {
                CleanUpJob job = new CleanUpJob { ecb = ecbs.CreateCommandBuffer().ToConcurrent() };
                var handle = job.Schedule(this, inputDeps);
                ecbs.AddJobHandleForProducer(handle);
                return handle;
            }
        }
    }
}