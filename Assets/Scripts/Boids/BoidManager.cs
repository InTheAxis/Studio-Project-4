using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Rendering;

public class BoidManager : ComponentSystem
{
    private BoidManagerData managerData;
    private bool receivedData = false;
    private int currentFrame = 0;
    private const int split = 1;

    protected override void OnUpdate()
    {
        if (!receivedData)
        {
            Entities.ForEach((Entity ent, ref BoidManagerData data) =>
            {
                managerData = data;
                receivedData = true;
                EntityManager.DestroyEntity(ent);
            });
        }
        Run();

    }
    public void Run()
    {
        // get monster data
        if (!receivedData)
            return;
        MonsterData md;
        bool bmd = false;
        Vector3 mPos = Vector3.zero;
        Entities.ForEach((ref MonsterData data) =>
        {
            md = data;
            mPos = md.monsterPos;
            bmd = true;
        });
        if (!bmd)
            return;
        //
        ++currentFrame;
        if (currentFrame >= split)
        {
            // Main loop
            Entities.ForEach((Entity boid, ref BoidData data, ref Translation trans) =>
            {
                Flock(boid, ref data, trans);
            });
            currentFrame = 0;
        }

        UpdatePos(mPos);
    }
    public void CleanUp()
    {
        Entities.ForEach((Entity ent, ref BoidData data) =>
        {
            EntityManager.DestroyEntity(ent);
        });
    }

    private void UpdatePos(Vector3 start)
    {
        start.y += 10;
        Entities.ForEach((ref BoidData data, ref Translation translation) =>
        {
            if (translation.Value.y < managerData.minHeight|| translation.Value.y > managerData.maxHeight)
            {
                translation.Value = start;
            }
            if (Vector3.Distance(translation.Value, start) > managerData.range)
            {
                translation.Value = start;
                data.vel = UnityEngine.Random.insideUnitSphere;
            }
            // update position
            Vector3 currentPos = translation.Value;
            translation.Value = currentPos + data.vel * managerData.speed;
        });
    }

    private void Flock(Entity boidA, ref BoidData dataA, Translation transA)
    {
        Vector3 posA = transA.Value;
        Vector3 alignDir = Vector3.zero;
        Vector3 cohesePos = Vector3.zero;
        Vector3 separatePos = Vector3.zero;
        int numInRange = 0;
        int numInSeparate = 0;
        Entities.ForEach((Entity boidB, ref BoidData dataB, ref Translation transB) =>
        {
            Vector3 posB = transB.Value;
            if (boidA != boidB)
            {
                // align and cohese
                if (managerData.align || managerData.cohese)
                {
                    if (Vector3.Distance(posA, posB) < managerData.viewRadius)
                    {
                        ++numInRange;

                        alignDir += dataB.vel;
                        cohesePos += posB;
                    }
                }
                if (managerData.separate)
                {
                    //separate
                    if (Vector3.Distance(posA, posB) < managerData.separateRadius)
                    {
                        ++numInSeparate;
                        separatePos += posB;
                    }
                }
            }
        });
        if (managerData.align && numInRange > 0)
        {
            alignDir /= numInRange; // average dir
            dataA.vel += alignDir * managerData.alignRate; // update vel
        }
        if (managerData.cohese && numInRange > 0)
        {
            cohesePos /= numInRange; // average pos
            Vector3 dir = cohesePos - posA;
            dataA.vel += dir * managerData.coheseRate; // update vel
        }
        if (managerData.separate && numInSeparate > 0)
        {
            separatePos /= numInSeparate; // average pos
            Vector3 dir = posA - separatePos;
            dataA.vel += dir * managerData.separateRate; // update vel
        }
        dataA.vel.Normalize(); // normalise vel
        if (transA.Value.y < managerData.avoidGroundHeight)
        {
            dataA.vel += Vector3.up * managerData.avoidGroundRate; // update vel
        }
        if (transA.Value.y > managerData.avoidCeilingHeight)
        {
            dataA.vel -= Vector3.up * managerData.avoidGroundRate; // update vel
        }
        dataA.vel.Normalize(); // normalise vel
    }
}

//public class BoidManagerJobSystem : JobComponentSystem
//{


//    //
//    public EntityCommandBuffer.Concurrent ecb;
//    [RequireComponentTag(typeof(BoidData))]
//    private struct BoidManagerJob : IJobForEachWithEntity<Translation, BoidData>
//    {
//        private BoidManagerData managerData;
//        private bool receivedData ;
//        private int currentFrame;
//        private const int split = 1;

//        public struct Data
//        {
//            public int Length;
//            public ComponentDataFromEntity<BoidData> boid;
//            public ComponentDataFromEntity<BoidData,Translation> boid;
//        }


//        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<BoidManagerData> dataArray;
//        [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<MonsterData> monsterDataArray;
//        public void Execute(Entity boid, int index, ref Translation trans, ref BoidData data)
//        {
//            if (!receivedData)
//            {
//                //Entities.ForEach((Entity ent, ref BoidManagerData data) =>
//                for(int i =0;i < dataArray.Length; ++i)
//                {
//                    managerData = dataArray[i];
//                    receivedData = true;
//                    // EntityManager.DestroyEntity(dataArray[i]);
//                };
//            }
//            Run(boid, ref data, ref trans);
//        }
//        //
//        //
//        //
//        public void Run(Entity boidA, ref BoidData dataA, ref Translation transA)
//        {
//            // get monster data
//            if (!receivedData)
//                return;
//            MonsterData md;
//            bool bmd = false;
//            Vector3 mPos = Vector3.zero;
//            for (int i = 0; i < monsterDataArray.Length; ++i)
//            {
//                md = monsterDataArray[i];
//                mPos = md.monsterPos;
//                bmd = true;
//            };
//            if (!bmd)
//                return;
//            //
//            ++currentFrame;
//            if (currentFrame >= split)
//            {
//                // Main loop
//                for (int i = 0; i < boidArray.Length; ++i)
//                {
//                    Flock(boidA, ref dataA, transA);
//                    UpdatePos(mPos,ref transA,ref dataA);
//                }; 
//                currentFrame = 0;
//            }

//        }
//        public void CleanUp()
//        {
//            //Entities.ForEach((Entity ent, ref BoidData data) =>
//            //{
//            //    EntityManager.DestroyEntity(ent);
//            //});
//        }

//        private void UpdatePos(Vector3 start, ref Translation translation, ref BoidData data)
//        {
//            start.y += 10;

//                if (translation.Value.y < managerData.minHeight || translation.Value.y > managerData.maxHeight)
//                {
//                    translation.Value = start;
//                }
//                if (Vector3.Distance(translation.Value, start) > managerData.range)
//                {
//                    translation.Value = start;
//                    data.vel = UnityEngine.Random.insideUnitSphere;
//                }
//                // update position
//                Vector3 currentPos = translation.Value;
//                translation.Value = currentPos + data.vel * managerData.speed;

//        }

//        private void Flock(Entity boidA, ref BoidData dataA, Translation transA)
//        {
//            Vector3 posA = transA.Value;
//            Vector3 alignDir = Vector3.zero;
//            Vector3 cohesePos = Vector3.zero;
//            Vector3 separatePos = Vector3.zero;
//            int numInRange = 0;
//            int numInSeparate = 0;
//            Entities.ForEach((Entity boidB, ref BoidData dataB, ref Translation transB) =>
//            {
//                Vector3 posB = transB.Value;
//                if (boidA != boidB)
//                {
//                    // align and cohese
//                    if (managerData.align || managerData.cohese)
//                    {
//                        if (Vector3.Distance(posA, posB) < managerData.viewRadius)
//                        {
//                            ++numInRange;

//                            alignDir += dataB.vel;
//                            cohesePos += posB;
//                        }
//                    }
//                    if (managerData.separate)
//                    {
//                        //separate
//                        if (Vector3.Distance(posA, posB) < managerData.separateRadius)
//                        {
//                            ++numInSeparate;
//                            separatePos += posB;
//                        }
//                    }
//                }
//            });
//            if (managerData.align && numInRange > 0)
//            {
//                alignDir /= numInRange; // average dir
//                dataA.vel += alignDir * managerData.alignRate; // update vel
//            }
//            if (managerData.cohese && numInRange > 0)
//            {
//                cohesePos /= numInRange; // average pos
//                Vector3 dir = cohesePos - posA;
//                dataA.vel += dir * managerData.coheseRate; // update vel
//            }
//            if (managerData.separate && numInSeparate > 0)
//            {
//                separatePos /= numInSeparate; // average pos
//                Vector3 dir = posA - separatePos;
//                dataA.vel += dir * managerData.separateRate; // update vel
//            }
//            dataA.vel.Normalize(); // normalise vel
//            if (transA.Value.y < managerData.avoidGroundHeight)
//            {
//                dataA.vel += Vector3.up * managerData.avoidGroundRate; // update vel
//            }
//            if (transA.Value.y > managerData.avoidCeilingHeight)
//            {
//                dataA.vel -= Vector3.up * managerData.avoidGroundRate; // update vel
//            }
//            dataA.vel.Normalize(); // normalise vel
//        }
//    }
//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
        
//    }
//    //
//    //
//    //
    
//}