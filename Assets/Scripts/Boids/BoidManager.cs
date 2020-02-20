using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

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
            });
        }

        if (!receivedData)
            return;
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

        UpdatePos();
    }

    private void UpdatePos()
    {
        Entities.ForEach((ref BoidData data, ref Translation translation) =>
        {
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
    }

    //private float3 Normalise(float3 v)
    //{
    //    return v / Mag(v);
    //}

    //private float SqrDistanceBetween(float3 a, float3 b)
    //{
    //    float x = a.x - b.x;
    //    float y = a.y - b.y;
    //    float z = a.z - b.z;
    //    float3 f = new float3(x, y, z);
    //    return SqrMag(f);
    //}

    //private float SqrMag(float3 f)
    //{
    //    return f.x * f.x + f.y * f.y + f.z * f.z;
    //}

    //private float Mag(float3 f)
    //{
    //    return math.sqrt(f.x * f.x + f.y * f.y + f.z * f.z);
    //}

    //private float DistanceBetween(float3 a, float3 b)
    //{
    //    return math.sqrt(SqrDistanceBetween(a, b));
    //}
}