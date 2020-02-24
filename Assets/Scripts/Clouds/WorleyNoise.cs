using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorleyNoise
{
    private List<Vector3> points = new List<Vector3>();
    private List<Vector3> originPoints = new List<Vector3>();
    private int numPoints = 10;
    private float scale = 4;
    // private Vector3 offset;

    public List<Vector3> GetPoints()
    {
        return new List<Vector3>(points);
    }

    public void Generate(int numPoints = 5, float scale = 1)
    {
        this.scale = scale;
        this.numPoints = numPoints;
        points.Clear();
        originPoints.Clear();
        for (int i = 0; i < numPoints; ++i)
        {
            Vector3 point = new Vector3(Random.value, Random.value, Random.value) * scale;
            originPoints.Add(point);
            Vector3 x = new Vector3(1, 0, 0) * scale;
            Vector3 y = new Vector3(0, 1, 0) * scale;
            Vector3 z = new Vector3(0, 0, 1) * scale;
            originPoints.Add(point + x);
            originPoints.Add(point + y);
            originPoints.Add(point + z);

            originPoints.Add(point - x);
            originPoints.Add(point - y);
            originPoints.Add(point - z);

            originPoints.Add(point + x + y);
            originPoints.Add(point + x - y);

            originPoints.Add(point - x + y);
            originPoints.Add(point - x - y);

            originPoints.Add(point + x + y + z);
            originPoints.Add(point + x - y + z);

            originPoints.Add(point - x + y + z);
            originPoints.Add(point - x - y + z);

            originPoints.Add(point + x + y - z);
            originPoints.Add(point + x - y - z);

            originPoints.Add(point - x + y - z);
            originPoints.Add(point - x - y - z);
        }
        points = new List<Vector3>(originPoints);
    }

    public void SetOffset(Vector3 offset)
    {
        // this.offset = offset;

        for (int i = 0; i < originPoints.Count; ++i)
        {
            points[i] = originPoints[i] + offset;
        }
        while (MovePoints()) { };
    }

    private bool MovePoints()
    {
        bool anyPointOutside = false;
        for (int i = 0; i < points.Count; ++i)
        {
            Vector3 point = points[i];
            if (point.x > scale)
            {
                point.x -= scale;
                anyPointOutside = true;
            }
            if (point.x < 0)
            {
                point.x += scale;
                anyPointOutside = true;
            }
            if (point.y > scale)
            {
                point.y -= scale;
                anyPointOutside = true;
            }
            if (point.y < 0)
            {
                point.y += scale;
                anyPointOutside = true;
            }
            if (point.z > scale)
            {
                point.z -= scale;
                anyPointOutside = true;
            }
            if (point.z < 0)
            {
                point.z += scale;
                anyPointOutside = true;
            }
            points[i] = point;
        }
        return anyPointOutside;
    }

    public Texture2D GenerateTexture(int resolution, float layer, bool invert)
    {
        Texture2D tex = new Texture2D(resolution, resolution);
        tex.filterMode = FilterMode.Point;
        Color[] colors = new Color[resolution * resolution];
        for (int x = 0; x < resolution; ++x)
        {
            for (int y = 0; y < resolution; ++y)
            {
                Vector3 pos = new Vector3(x, y, layer) / resolution * scale;
                pos.z = layer * scale;
                colors[y * resolution + x] = GetPixelColor(pos, invert);
            }
        }
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    public Texture3D GenerateTexture3D(int resolution, float layer, bool invert)
    {
        Texture3D tex = new Texture3D(resolution, resolution, resolution, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        tex.filterMode = FilterMode.Point;
        Color[] colors = new Color[resolution * resolution * resolution];
        for (int z = 0; z < resolution; ++z)
        {
            for (int x = 0; x < resolution; ++x)
            {
                for (int y = 0; y < resolution; ++y)
                {
                    Vector3 pos = new Vector3(x, y, z) / resolution * scale;
                    //pos.z = layer;
                    colors[z * resolution * resolution + y * resolution + x] = GetPixelColor(pos, invert);
                }
            }
        }
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }

    private Color GetPixelColor(Vector3 pos, bool invert)
    {
        float minDist = DistToNearestPoint(pos);
        minDist = Mathf.Min(1, minDist); // max val as 1
        if (invert)
            return new Color(1 - minDist, 1 - minDist, 1 - minDist);
        else
            return new Color(minDist, minDist, minDist);
    }

    private float DistToNearestPoint(Vector3 pos)
    {
        float minDist = float.MaxValue;

        foreach (Vector3 point in points)
        {
            float dist = Vector3.Distance(point, pos);
            if (dist < minDist)
                minDist = dist;
        }
        return minDist;
    }
}