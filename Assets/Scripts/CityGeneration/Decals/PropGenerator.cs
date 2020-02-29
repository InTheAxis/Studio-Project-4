using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;
using static Unity.Mathematics.math;

public struct DecalTag : IComponentData { }

public class PropGenerator : ComponentSystem
{
    public bool generated = false;
    private PoissonGenerator poisson = new PoissonGenerator();

    public void CleanUp()
    {
        Entities.ForEach((Entity ent, ref DecalTag data) =>
        {
            EntityManager.DestroyEntity(ent);
        });
        generated = false;
    }

    protected override void OnUpdate()
    {
        if (generated)
            return;
        Entities.ForEach((Entity ent, ref DecalData data) =>
        {
            EntityManager.DestroyEntity(ent);
            generated = true;
            poisson.GenerateDensity(data.density);
            poisson.Scale(data.range);
            foreach (PoissonPoint point in poisson.GetPoints())
            {
                Entity entRef = data.decal0;
                int index = UnityEngine.Random.Range(0, data.numRock);
                switch (index)
                {
                    case 0:
                        entRef = data.decal0;
                        break;

                    case 1:
                        entRef = data.decal1;
                        break;

                    case 2:
                        entRef = data.decal2;
                        break;

                    case 3:
                        entRef = data.decal3;
                        break;

                    case 4:
                        entRef = data.decal4;
                        break;

                    case 5:
                        entRef = data.decal5;
                        break;

                    case 6:
                        entRef = data.decal6;
                        break;

                    case 7:
                        entRef = data.decal7;
                        break;

                    case 8:
                        entRef = data.decal8;
                        break;

                    case 9:
                        entRef = data.decal9;
                        break;

                    case 10:
                        entRef = data.decal10;
                        break;

                    case 11:
                        entRef = data.decal11;
                        break;

                    case 12:
                        entRef = data.decal12;
                        break;
                }
                Entity spawned = EntityManager.Instantiate(entRef);
                EntityManager.AddComponent<DecalTag>(spawned);
                EntityManager.SetComponentData(spawned, new Translation { Value = new float3(point.pos) });
                EntityManager.SetComponentData(spawned, new Rotation { Value = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0) });
            }
        });
    }
}