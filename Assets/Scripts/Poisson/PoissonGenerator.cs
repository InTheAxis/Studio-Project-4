using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonGenerator
{
    public List<Vector3> points = new List<Vector3>();

    //  private bool generated = false;
    private int density = -1;

    private float buffer = -1;

    private bool threeDimensions = false;
    private bool centered = false;

    public PoissonGenerator()
    {
    }

    //public PoissonGenerator(int density, float buffer, bool threeDimensions = false)
    //{
    //    this.threeDimensions = threeDimensions;
    //    this.density = density;
    //    this.buffer = buffer;
    //    Generate();
    //}
    public void Scale(float scale)
    {
        for (int i = 0; i < points.Count; ++i)
        {
            points[i] = points[i] * scale;
        }
    }

    public List<Vector3> GetPoints()
    {
        return points;
    }

    public void SetToXZ()
    {
        for (int i = 0; i < points.Count; ++i)
        {
            points[i] = new Vector3(points[i].x, 0, points[i].y);
        }
    }

    public void Set(int density, float buffer, bool threeDimensions = false, bool centered = false)
    {
        this.threeDimensions = threeDimensions;
        this.density = density;
        this.buffer = buffer;
        this.centered = centered;
    }

    public void Generate()
    {
        points.Clear();
        int numGenerated = 0;
        int attempts = 0;
        int MAX_ATTEMPTS = density * 2;
        if (centered)
        {
            points.Add(Vector3.zero);
            ++numGenerated;
        }
        while (numGenerated < density && attempts < MAX_ATTEMPTS)
        {
            ++attempts;

            Vector3 point;
            if (threeDimensions)
                point = Random.insideUnitSphere;
            else
                point = Random.insideUnitCircle;
            bool add = true;
            foreach (Vector3 checkPoint in points)
            {
                if (Vector3.Distance(checkPoint, point) < buffer)
                {
                    add = false;
                    break;
                }
            }
            if (add)
                points.Add(point);
        }
    }
}