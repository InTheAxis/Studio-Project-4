using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiVertice
{
    // public List<Color> colors = new List<Color>();
    public Vector3 pos = new Vector3();

    public List<int> centroidIndexes;
}

public class Voronoi
{
    // to be get from the user
    private List<Vector3> xzCentroids = new List<Vector3>();

    private bool generated = false;

    // only for use by generator
    private Vector2Int[] mCentroids;

    private Color[] regionColors;
    private Vector2Int resolution;
    private int density;
    private List<VoronoiVertice> voronoiPoints = new List<VoronoiVertice>();
    private List<VoronoiVertice> centralPoints = new List<VoronoiVertice>();

    public Color[] pixelColors;
    public int[] indexGrid;

    private PoissonGenerator poisson = new PoissonGenerator();

    public bool IsGenerated()
    {
        return generated;
    }

    public List<Vector3> GetCentroids()
    {
        return xzCentroids;
    }
    public PoissonGenerator GetPoisson()
    {
        return poisson;
    }

    public void Scale(float scale)
    {
        for (int i = 0; i < voronoiPoints.Count; ++i)
        {
            voronoiPoints[i].pos = voronoiPoints[i].pos * scale;
        }
        for (int i = 0; i < density; ++i)
        {
            mCentroids[i] = mCentroids[i] * (int)scale;
        }
        for (int i = 0; i < density; ++i)
        {
            xzCentroids[i] = xzCentroids[i] * (int)scale;
        }
    }

    private void SetToXZ()
    {
        xzCentroids.Clear();
        for (int i = 0; i < voronoiPoints.Count; ++i)
        {
            VoronoiVertice vertice = voronoiPoints[i];
            vertice.pos = new Vector3(vertice.pos.x, 0, vertice.pos.y);
            voronoiPoints[i] = vertice;
        }

        foreach (Vector2Int point in mCentroids)
        {
            xzCentroids.Add(new Vector3(point.x, 0, point.y));
        }
    }

    public List<VoronoiVertice> GetCentral()
    {
        return centralPoints;
    }

    private void CenterPoints()
    {
        foreach (VoronoiVertice vertice in voronoiPoints)
        {
            vertice.pos = new Vector3(vertice.pos.x / resolution.x - 0.5f, vertice.pos.y / resolution.y - 0.5f, 0);
        }
        for (int i = 0; i < density; ++i)
        {
            mCentroids[i] = new Vector2Int((int)(mCentroids[i].x / resolution.x - 0.5f), (int)(mCentroids[i].y / resolution.y - 0.5f));
        }
    }

    public void Generate(Vector2Int resolution, int density, float centerBuffer)
    {
        generated = true;
        this.resolution = resolution;
        this.density = density;
        mCentroids = new Vector2Int[density];
        regionColors = new Color[density];
        poisson.ClearInjected();

        poisson.Inject(new PoissonPoint(Vector2.zero, centerBuffer));
        bool generatedPoisson = poisson.Generate(density, 0.00f);
        List<PoissonPoint> poissonList = poisson.GetPoints();
        for (int i = 0; i < density; ++i)
        {
            // centroids[i] = new Vector2Int(Random.Range(0, resolution.x), Random.Range(0, resolution.y));
            mCentroids[i] = new Vector2Int((int)(poissonList[i].pos.x * resolution.x / 2 + resolution.x / 2), (int)(poissonList[i].pos.z * resolution.y / 2 + resolution.y / 2));
            regionColors[i] = Random.ColorHSV();
        }
        pixelColors = new Color[resolution.x * resolution.y];
        indexGrid = new int[resolution.x * resolution.y];
        for (int x = 0; x < resolution.x; ++x)
        {
            for (int y = 0; y < resolution.y; ++y)
            {
                int index = y * resolution.x + x;
                pixelColors[index] = regionColors[GetClosestCentroidIndex(new Vector2Int(x, y), mCentroids)];
                indexGrid[index] = GetClosestCentroidIndex(new Vector2Int(x, y), mCentroids);
            }
        }
        // centralCentroid = mCentroids[0];
        SaveVoronoiPoints();
        CenterPoints();
        SetToXZ();
        SaveCentralPoints();
    }

    private void SaveCentralPoints()
    {
        centralPoints.Clear();
        foreach (VoronoiVertice vertice in voronoiPoints)
        {
            if (vertice.centroidIndexes.Contains(0))
                centralPoints.Add(vertice);
        }
    }

    public Texture2D GetVoronoiTexture()
    {
        return ColorArrayToTexture2D(resolution, pixelColors);
    }

    private void SaveVoronoiPoints()
    {
        List<VoronoiVertice> points = new List<VoronoiVertice>();
        for (int x = 0; x < resolution.x; ++x)
        {
            for (int y = 0; y < resolution.y; ++y)
            {
                int index = y * resolution.x + x;
                // Color currentColor = pixelColors[index];
                int currentIndex = indexGrid[index];
                // List<Color> containingColors = new List<Color>();
                List<int> containingIndexes = new List<int>();
                //containingColors.Add(currentColor);
                containingIndexes.Add(currentIndex);
                int diffCounter = 0;
                if (y != resolution.y - 1)
                {
                    //Color next = pixelColors[(y + 1) * resolution.x + (x)];
                    int nextIndex = indexGrid[(y + 1) * resolution.x + (x)];
                    if (nextIndex != currentIndex && !containingIndexes.Contains(nextIndex))
                    {
                        diffCounter++;
                        //containingColors.Add(next);
                        containingIndexes.Add(nextIndex);
                    }
                }
                if (y != 0)
                {
                    //Color next = pixelColors[(y - 1) * resolution.x + (x)];
                    int nextIndex = indexGrid[(y - 1) * resolution.x + (x)];
                    if (nextIndex != currentIndex && !containingIndexes.Contains(nextIndex))
                    {
                        diffCounter++;
                        //containingColors.Add(next);
                        containingIndexes.Add(nextIndex);
                    }
                }
                if (x != 0)
                {
                    //Color next = pixelColors[(y) * resolution.x + (x - 1)];
                    int nextIndex = indexGrid[(y) * resolution.x + (x - 1)];
                    if (nextIndex != currentIndex && !containingIndexes.Contains(nextIndex))
                    {
                        diffCounter++;
                        //containingColors.Add(next);
                        containingIndexes.Add(nextIndex);
                    }
                }
                if (x != resolution.x - 1)
                {
                    //Color next = pixelColors[(y) * resolution.x + (x + 1)];
                    int nextIndex = indexGrid[(y) * resolution.x + (x + 1)];
                    if (nextIndex != currentIndex && !containingIndexes.Contains(nextIndex))
                    {
                        diffCounter++;
                        //containingColors.Add(next);
                        containingIndexes.Add(nextIndex);
                    }
                }
                if (diffCounter > 1)
                {
                    VoronoiVertice vertice = new VoronoiVertice();
                    //vertice.colors = containingColors;
                    vertice.pos = new Vector2(x, y);
                    vertice.centroidIndexes = containingIndexes;
                    points.Add(vertice);
                }
                else if (x == resolution.x - 1 || x == 0 || y == resolution.y - 1 || y == 0)
                {
                    if (diffCounter == 1)
                    {
                        VoronoiVertice vertice = new VoronoiVertice();
                        //vertice.colors = containingColors;
                        vertice.pos = new Vector2(x, y);
                        vertice.centroidIndexes = containingIndexes;
                        points.Add(vertice);
                    }
                }
            }
        }

        for (int firstPass = 0; firstPass < points.Count - 1; ++firstPass)
        {
            const int range = 2;
            Vector2 pointA = points[firstPass].pos;
            for (int secondPass = firstPass + 1; secondPass < points.Count; ++secondPass)
            {
                Vector2 pointB = points[secondPass].pos;
                if (Vector2.Distance(pointA, pointB) <= range)
                {
                    // remove point B
                    if ((points[secondPass].centroidIndexes.Contains(0)))
                    {
                        points.RemoveAt(firstPass);
                        --firstPass;
                        break;
                    }
                    else
                    {
                        points.RemoveAt(secondPass);
                        --secondPass;
                    }
                }
            }
        }
        voronoiPoints = points;
    }

    public List<VoronoiVertice> GetVoronoiPoints()
    {
        return voronoiPoints;
    }

    private int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
    {
        float smallestDist = float.MaxValue;
        int closestIndex = -1;
        int i = 0;
        foreach (Vector2Int centroid in centroids)
        {
            float distance = Vector2.Distance(pixelPos, centroid);
            if (distance <= smallestDist)
            {
                smallestDist = distance;
                closestIndex = i;
            }
            ++i;
        }
        return closestIndex;
    }

    private Texture2D ColorArrayToTexture2D(Vector2Int res, Color[] colors)
    {
        Texture2D tex = new Texture2D(res.x, res.y);
        tex.filterMode = FilterMode.Point;
        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }
}