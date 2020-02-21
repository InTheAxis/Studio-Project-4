using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public class PropGenerator : ComponentSystem
{
    public bool generated = false;
    private PoissonGenerator poisson = new PoissonGenerator();

    protected override void OnUpdate()
    {
        if (generated)
            return;
        Entities.ForEach((ref DecalData data) =>
        {
            generated = true;
            poisson.GenerateDensity(data.density);
            poisson.Scale(data.range);
            foreach (PoissonPoint point in poisson.GetPoints())
            {
                Entity entRef = data.rockA;
                int index = UnityEngine.Random.Range(0, data.numRock);
                switch (index)
                {
                    case 0:
                        entRef = data.rockA;
                        break;

                    case 1:
                        entRef = data.rockB;
                        break;

                    case 2:
                        entRef = data.rockC;
                        break;

                    case 3:
                        entRef = data.rockD;
                        break;

                    case 4:
                        entRef = data.rockE;
                        break;
                }
                Entity spawned = EntityManager.Instantiate(entRef);
                EntityManager.SetComponentData(spawned, new Translation { Value = new float3(point.pos) });
                EntityManager.SetComponentData(spawned, new Rotation { Value = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0) });
            }
        });
    }
}