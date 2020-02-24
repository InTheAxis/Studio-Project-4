using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBuildingGenerator : Generator
{
    [SerializeField]
    private CellGenerator cellGenerator;

    [SerializeField]
    public CityScriptable cityScriptable;

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
        foreach (BuildingCell cell in cellGenerator.GetCells())
        {
            GameObject buildingRef = cityScriptable.SelectMesh(cell.radius);
            if (buildingRef == null)
                continue;
            float buildingRadius = buildingRef.GetComponent<ProceduralBuilding>().GetRadius();
            Vector3 pos = cell.pos;
            pos += cell.offSetDir * buildingRadius;
            pos.y += 0.2f;
            // Instantitate
            GameObject building = InstantiateHandler.mInstantiate(buildingRef, pos, cell.rot, transform, "Environment");
            building.GetComponent<ProceduralBuilding>().GenerateRandom();
            //building.transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
        }
    }
}