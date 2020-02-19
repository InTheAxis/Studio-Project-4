using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : Generator
{
    private PoissonGenerator poisson = new PoissonGenerator();
    public CityScriptable cityScriptable;

    [Range(0, 1000)]
    public int density = 5;

    [Range(0, 1)]
    public float buffer = 0.1f;

    public override void Clear()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public override void Generate()
    {
        poisson.Generate(density, buffer);
        poisson.Scale(scale/2);
        foreach (PoissonPoint pos in poisson.GetPoints())
        {
            GameObject buildingRef = cityScriptable.SelectMesh();
            GameObject building = InstantiateHandler.mInstantiate(buildingRef, transform, "Environment");
            building.transform.position = pos.pos;
            building.AddComponent<BuildingCollisionScript>();
            building.GetComponent<ProceduralBuilding>().GenerateRandom();
            building.transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Transform trans in transform)
        {
            Gizmos.DrawWireSphere(trans.position, buffer * scale);
        }
    }
}