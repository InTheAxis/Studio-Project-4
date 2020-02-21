using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : Generator
{
    private PoissonGenerator poisson = new PoissonGenerator();
    public CityScriptable cityScriptable;
    public TowerGenerator towerGenerator;
    // public float roadSearchRange = 15;

    [Range(0, 1000)]
    public int density = 5;

    [Range(0, 1)]
    public float buffer = 0.1f;

    [Range(0, 1)]
    public float centerBuffer = 0.1f;

    [Range(0, 1)]
    public float towerBuffer = 0.1f;

    public override void Clear()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public override void Generate()
    {
        Clear();
        poisson.ClearInjected();
        poisson.Inject(new PoissonPoint(Vector2.zero, centerBuffer));
        foreach (PoissonPoint point in towerGenerator.GetPoisson().GetPoints(1))
        {
            poisson.Inject(new PoissonPoint(point.pos / scale, towerBuffer));
        }
        poisson.Generate(density, buffer);
        poisson.Scale(scale);
        foreach (PoissonPoint pos in poisson.GetPoints(towerGenerator.GetPoisson().GetPoints(1).Count + 1))
        {
            Vector3 vpos = pos.pos;
            vpos.y += 0.1f;
            GameObject buildingRef = cityScriptable.SelectMesh();
            GameObject building = InstantiateHandler.mInstantiate(buildingRef, vpos, Quaternion.identity, transform, "Environment");
            building.GetComponent<ProceduralBuilding>().GenerateRandom();
            building.transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
            // check for road
            Collider[] colls = Physics.OverlapSphere(new Vector3(building.transform.position.x, 0, building.transform.position.z), buffer * scale);
            foreach (Collider col in colls)
            {
                if (col.tag == "Road")
                {
                    building.SetActive(false);
                    break;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Transform trans in transform)
        {
            Gizmos.DrawWireSphere(trans.position, buffer * scale);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, scale * centerBuffer);
        foreach (PoissonPoint point in towerGenerator.GetPoisson().GetPoints(1))
        {
            // poisson.Inject(new PoissonPoint(point.pos, towerBuffer));
            Gizmos.DrawWireSphere(point.pos, towerBuffer * scale);
        }
    }
}