using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PoissonPoint
{
    public PoissonPoint(Vector3 pos, float r)
    {
        radius = r;
        this.pos = new Vector3(pos.x, pos.z, 0);
    }

    public PoissonPoint(Vector2 pos, float r)
    {
        radius = r;
        this.pos = pos;
    }

    public float radius;
    public Vector3 pos;
}

public class PoissonGenerator
{
    private List<PoissonPoint> points = new List<PoissonPoint>();
    private List<PoissonPoint> xzPoints = new List<PoissonPoint>();
    private List<PoissonPoint> injected = new List<PoissonPoint>();
    private float scale = 1;

    public void Inject(PoissonPoint poissonPoint)
    {
        injected.Add(poissonPoint);
    }

    public void ClearInjected()
    {
        injected.Clear();
    }

    public void Scale(float scale)
    {
        this.scale = scale;
        for (int i = 0; i < points.Count; ++i)
        {
            PoissonPoint scaled = points[i];
            scaled.pos = points[i].pos * scale;
            scaled.radius *= scale;
            points[i] = scaled;
        }
        SetToXZ();
    }

    public List<PoissonPoint> GetPoints()
    {
        return xzPoints;
    }

    public List<PoissonPoint> GetPoints(int startIndex)
    {
        if (xzPoints.Count < startIndex)
        {
            return xzPoints;
        }
        return xzPoints.GetRange(startIndex, xzPoints.Count - startIndex);
    }

    private void SetToXZ()
    {
        xzPoints.Clear();
        for (int i = 0; i < points.Count; ++i)
        {
            PoissonPoint point = points[i];
            Vector3 v3Point = new Vector3(point.pos.x, 0, point.pos.y);
            point.pos = v3Point;
            xzPoints.Add(point);
        }
    }

    private void InjectIntoMain()
    {
        foreach (PoissonPoint point in injected)
        {
            points.Add(point);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="density"></param>
    /// <param name="radius"></param>
    /// <returns>return true if density was generated. false if less then the density was generated</returns>
    public bool Generate(int density, float radius, int maxAttempt = 4)
    {
        points.Clear();
        InjectIntoMain();
        int numGenerated = 0;
        int attempts = 0;
        int MAX_ATTEMPTS = density * maxAttempt;

        while (numGenerated < density && attempts < MAX_ATTEMPTS)
        {
            ++attempts;

            Vector2 point;
            point = Random.insideUnitCircle;
            bool add = true;
            foreach (PoissonPoint checkPoint in points)
            {
                if (Vector3.Distance(checkPoint.pos, point) < radius + checkPoint.radius)
                {
                    add = false;
                    break;
                }
            }
            if (add)
            {
                attempts = 0;
                ++numGenerated;
                points.Add(new PoissonPoint(point, radius));
            }
        }
        SetToXZ();
        if (numGenerated < density)
            return false;
        return true;
    }

    /// <summary>
    /// Generate points based on density. Buffer between points are generated and multiplied by an optional multiplier.
    /// </summary>
    /// <param name="density"></param>
    /// <param name="multiplier"></param>
    /// <returns></returns>
    public bool GenerateDensity(int density, float multiplier = 0.8f)
    {
        float radius = 2 / (float)density * multiplier;
        return Generate(density, radius);
    }
}