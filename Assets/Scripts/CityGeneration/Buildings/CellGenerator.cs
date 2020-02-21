using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BuildingCell
{
    public BuildingCell(Vector3 pos, float size)
    {
        this.pos = pos;
        //this.min = min;
        //this.max = max;
        //this.minB = new Vector2(min.x)
        this.radius = size;
    }

    public Vector3 pos;
    //private Vector2 min;

    //private Vector2 minB;
    //private Vector2 max;
    //private Vector2 maxB;

    public float radius;
}

[System.Serializable]
public struct MinMax
{
    public float min;
    public float max;

    public float Get()
    {
        return Random.Range(min, max);
    }
}

public class CellGenerator : Generator
{
    private List<BuildingCell> cells = new List<BuildingCell>();

    //[SerializeField]
    //private float minOffset;

    //[SerializeField]
    //private float maxOffset;
    [SerializeField]
    private MinMax buffer;

    [SerializeField]
    private MinMax cellRadius;

    //[SerializeField]
    //private MinMax width;

    [SerializeField]
    private float initialSpace;

    [SerializeField]
    private RoadGenerator roadGenerator;

    public override void Clear()
    {
        cells.Clear();
    }

    public override void Generate()
    {
        Debug.Log("Begin cell generation.");
        Clear();
        foreach (RoadGenerator.RoadPath path in roadGenerator.GetRoadOuterPaths())
        {
            float currDist = 0;
            Vector3 dir = path.dir;
            Vector3 pDir = Vector3.Cross(dir, Vector3.up).normalized;
            Vector3 startPos = path.start + pDir * path.width / 2;
            Vector3 currPos = startPos;
            currPos += dir * initialSpace;
            currPos += dir * buffer.Get();
            while (currDist < path.Length())
            {
                float currCellRadius = cellRadius.Get();
                //float cellWidth = width.Get();
                float spacing = buffer.Get();
                currDist += currCellRadius;
                currPos = currDist * dir + startPos;
                //Vector3 end = currPos + dir * cellLength;
                //Vector3 cellMax = end + pDir * cellWidth;

                // create cell
                cells.Add(new BuildingCell(currPos, currCellRadius));
                currDist += spacing + currCellRadius;
            }
        }
        Debug.Log("Generated Cells.");
    }

    private void OnDrawGizmos()
    {
        if (!gizmosEnabled)
            return;
        foreach (BuildingCell cell in cells)
        {
            Gizmos.DrawWireSphere(cell.pos, cell.radius);
        }
    }
}