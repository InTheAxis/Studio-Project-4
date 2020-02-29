using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class BoidSpawner : ComponentSystem
{
    private bool generated = false;
    private double lastSpawn = 0;
    private int numGenerate = 0;
    public void Reset()
    {
        generated = false;
        numGenerate = 0;
        lastSpawn = 0;
    }

    protected override void OnUpdate()
    {
        if (generated)
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
        Entities.ForEach((Entity ent, ref BoidSpawnerData data) =>
        {
            //for (int i = 0; i < data.numSpawn; ++i)
            {
                if (Time.ElapsedTime < lastSpawn + data.buffer)
                    return;
                lastSpawn = Time.ElapsedTime;
                Vector3 offset = Random.insideUnitSphere * data.range; 
                Vector3 pos = mPos + offset;
                //pos.x += translation.Value.x;
                pos.y += 20;
                //pos.z += translation.Value.z;
                Entity spawned = EntityManager.Instantiate(data.boidRef);
                Vector3 initialVel = Random.insideUnitSphere;
                EntityManager.SetComponentData(spawned, new Translation { Value = pos });
                EntityManager.SetComponentData(spawned, new BoidData(initialVel) { });
                ++numGenerate;
            }
            if (numGenerate == data.numSpawn)
            { 
                generated = true;
                EntityManager.DestroyEntity(ent);
            }

        });
    }
}