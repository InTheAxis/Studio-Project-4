using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : Generator
{
    private PoissonGenerator poisson = new PoissonGenerator();

    [SerializeField]
    private CityGenerator cityGenerator;

    public TowerGenerator towerGenerator;
    public CellBuildingGenerator cellGenerator;
    // public float roadSearchRange = 15;

    [SerializeField]
    [Range(0, 1000)]
    private int density = 5;

    [SerializeField]
    [Min(0)]
    private float buffer = 0.1f;

    [SerializeField]
    [Min(0)]
    private float centerBuffer = 0.1f;

    [SerializeField]
    [Min(0)]
    private float towerBuffer = 0.1f;

    [SerializeField]
    private PlayerSpawnGenerator playerSpawnGenerator;

    [Min(0)]
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
        poisson.Inject(new PoissonPoint(Vector2.zero, centerBuffer / scale));
        // inject player/hunter
        foreach (Vector3 pos in PlayerSpawnGenerator.playerSpawnPos)
        {
            poisson.Inject(new PoissonPoint(pos / scale, playerBuffer / scale));
        }
        poisson.Inject(new PoissonPoint(PlayerSpawnGenerator.hunterSpawnPos / scale, playerBuffer / scale));
        // inject tower
        foreach (PoissonPoint point in towerGenerator.GetPoisson().GetPoints(1))
        {
            poisson.Inject(new PoissonPoint(point.pos / scale, towerBuffer / scale));
        }
        foreach (BuildingCell cell in cellGenerator.buildingCells)
        {
            poisson.Inject(new PoissonPoint(cell.pos / scale, cell.radius / scale));
        }
        bool success = poisson.Generate(density, buffer / scale);
        poisson.Scale(scale);
        foreach (PoissonPoint pos in poisson.GetPoints(towerGenerator.GetPoisson().GetPoints(1).Count + cellGenerator.buildingCells.Count + 1 + 5))
        {
            Vector3 vpos = pos.pos;
            vpos.y += 0.1f;
            // check for road
            bool emptySpot = true;
            GameObject buildingRef = cityGenerator.city.SelectMesh();
            if (!buildingRef)
            {
                Debug.LogError("building is null: " + gameObject.name);
                continue;
            }
            Collider[] colls = Physics.OverlapSphere(new Vector3(vpos.x, 0, vpos.z), buffer / 4, LayerMask.NameToLayer("Road"));
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
                GameObject building = InstantiateHandler.mInstantiate(buildingRef, vpos, Quaternion.identity, transform);
                building.GetComponent<ProceduralBuilding>().GenerateRandom();
                building.transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!gizmosEnabled)
            return;
        // all pos
        foreach (PoissonPoint pos in poisson.GetPoints(towerGenerator.GetPoisson().GetPoints(1).Count + cellGenerator.buildingCells.Count + 1))
        {
            Gizmos.DrawWireSphere(pos.pos, buffer);
        }
        // success pos
        Gizmos.color = Color.green;
        foreach (Transform trans in transform)
        {
            Gizmos.DrawWireSphere(trans.position, buffer);
        }
        // center
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, centerBuffer);
        // tower
        foreach (PoissonPoint point in towerGenerator.GetPoisson().GetPoints(1))
        {
            // poisson.Inject(new PoissonPoint(point.pos, towerBuffer));
            Gizmos.DrawWireSphere(point.pos, towerBuffer);
        }

        // player buffer
        Gizmos.color = Color.magenta;
        foreach (Vector3 pos in PlayerSpawnGenerator.playerSpawnPos)
        {
            Gizmos.DrawWireSphere(pos, playerBuffer);
        }
        Gizmos.DrawWireSphere(PlayerSpawnGenerator.hunterSpawnPos, playerBuffer);
    }
}