using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBuildingGenerator : Generator
{
    [SerializeField]
    private CityGenerator cityGenerator;

    [SerializeField]
    private CellGenerator cellGenerator;

    public List<BuildingCell> buildingCells { get; private set; } = new List<BuildingCell>();

    public override void Clear()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        buildingCells.Clear();
    }

    public override void Generate()
    {
        Clear();
        foreach (BuildingCell cell in cellGenerator.GetCells())
        {
            GameObject buildingRef = cityGenerator.city.SelectMesh(cell.radius);
            if (buildingRef == null)
                continue;
            float buildingRadius = buildingRef.GetComponent<ProceduralBuilding>().GetRadius();
            Vector3 pos = cell.pos;
            pos += cell.offSetDir * buildingRadius;
            pos.y += 2f;
            // Instantitate
            GameObject building = InstantiateHandler.mInstantiate(buildingRef, pos, cell.rot, transform, "Environment");
            building.GetComponent<ProceduralBuilding>().GenerateRandom();
            //building.transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
            buildingCells.Add(new BuildingCell(pos, buildingRadius, cell.offSetDir, cell.isLeft, cell.rot));
        }
    }
}