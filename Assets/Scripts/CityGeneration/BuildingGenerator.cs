using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : Generator
{
    private PoissonGenerator poisson = new PoissonGenerator();
    public CityScriptable cityScriptable;
    public TowerGenerator towerGenerator;
    public CellGenerator cellGenerator;
    // public float roadSearchRange = 15;

    [Range(0, 1000)]
    public int density = 5;

    [Range(0, 1)]
    public float buffer = 0.1f;

    [Range(0, 1)]
    public float centerBuffer = 0.1f;

    [Range(0, 1)]
    public float towerBuffer = 0.1f;

    [SerializeField]
    private PlayerSpawnGenerator playerSpawnGenerator;

    [Range(0, 1)]
    [SerializeField]
    private float playerBuffer;

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
        // inject player/hunter
        foreach (Vector3 pos in playerSpawnGenerator.playerSpawnPos)
        {
            poisson.Inject(new PoissonPoint(pos / scale, playerBuffer));
        }
        poisson.Inject(new PoissonPoint(playerSpawnGenerator.hunterSpawnPos / scale, playerBuffer));
        // inject tower
        foreach (PoissonPoint point in towerGenerator.GetPoisson().GetPoints(1))
        {
            poisson.Inject(new PoissonPoint(point.pos / scale, towerBuffer));
        }
        foreach (BuildingCell cell in cellGenerator.GetCells())
        {
            poisson.Inject(new PoissonPoint(cell.pos / scale, cell.radius / scale));
        }
        bool success = poisson.Generate(density, buffer);
        poisson.Scale(scale);
        foreach (PoissonPoint pos in poisson.GetPoints(towerGenerator.GetPoisson().GetPoints(1).Count + cellGenerator.GetCells().Count + 1 + 5))
        {
            Vector3 vpos = pos.pos;
            vpos.y += 0.2f;
            // check for road
            bool emptySpot = true;
            Collider[] colls = Physics.OverlapSphere(new Vector3(vpos.x, 0, vpos.z), buffer * scale);
            foreach (Collider col in colls)
            {
                if (col.tag == "Road")
                {
                    emptySpot = false;
                    break;
                }
            }
            if (emptySpot)
            {
                GameObject buildingRef = cityScriptable.SelectMesh();
                GameObject building = InstantiateHandler.mInstantiate(buildingRef, vpos, Quaternion.identity, transform, "Environment");
                building.GetComponent<ProceduralBuilding>().GenerateRandom();
                building.transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!gizmosEnabled)
            return;
        foreach (PoissonPoint pos in poisson.GetPoints(towerGenerator.GetPoisson().GetPoints(1).Count + cellGenerator.GetCells().Count + 1))
        {
            Gizmos.DrawWireSphere(pos.pos, buffer * scale);
        }
        Gizmos.color = Color.green;
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