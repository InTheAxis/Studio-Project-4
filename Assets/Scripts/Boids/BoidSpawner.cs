using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class BoidSpawner : ComponentSystem
{
    private bool generated = false;

    protected override void OnUpdate()
    {
        if (generated)
            return;
        Entities.ForEach((ref BoidSpawnerData data, ref Translation translation) =>
        {
            generated = true;
            for (int i = 0; i < data.numSpawn; ++i)
            {
                Vector3 pos = Random.insideUnitSphere * data.range;
                pos.x += translation.Value.x;
                pos.y += translation.Value.y;
                pos.z += translation.Value.z;
                Entity spawned = EntityManager.Instantiate(data.boidRef);
                Vector3 initialVel = Random.insideUnitSphere;
                EntityManager.SetComponentData(spawned, new Translation { Value = pos });
                EntityManager.SetComponentData(spawned, new BoidData(initialVel) { });
            }
        });
    }
}