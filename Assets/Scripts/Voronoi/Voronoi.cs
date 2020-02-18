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
    // public Vector3Int centralCentroid;
    private Vector3Int[] mCentroids;

    private Color[] regionColors;
    private Vector2Int resolution;
    private int density;
    private List<VoronoiVertice> voronoiPoints = new List<VoronoiVertice>();
    private List<VoronoiVertice> centralPoints = new List<VoronoiVertice>();
    private bool generated = false;

    public Color[] pixelColors;
    public int[] indexGrid;

    public bool IsGenerated()
    {
        return generated;
    }

    public Vector3Int[] GetCentroids()
    {
        return mCentroids;
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
    }

    public List<VoronoiVertice> GetCentral()
    {
        return centralPoints;
    }

    private void CenterPoints()
    {
        foreach (VoronoiVertice vertice in voronoiPoints)
        {
            vertice.pos = new Vector3(vertice.pos.x - resolution.x / 2, 0, vertice.pos.y - resolution.y / 2);
        }
        //foreach (VoronoiVertice vertice in centralPoints)
        //{
        //    vertice.pos = new Vector3(vertice.pos.x - resolution.x / 2, 0, vertice.pos.y - resolution.y / 2);
        //}
        for (int i = 0; i < density; ++i)
        {
            mCentroids[i] = new Vector3Int(mCentroids[i].x - resolution.x / 2, 0, mCentroids[i].y - resolution.y / 2);
        }
    }

    public void Generate(Vector2Int resolution, int density)
    {
        generated = true;
        this.resolution = resolution;
        this.density = density;
        mCentroids = new Vector3Int[density];
        regionColors = new Color[density];
        PoissonGenerator poissonGenerator = new PoissonGenerator();
        poissonGenerator.Set(density, 3.5f / (float)density, false, true);
        poissonGenerator.Generate();
        List<Vector3> poissonList = poissonGenerator.GetPoints();
        for (int i = 0; i < density; ++i)
        {
            // centroids[i] = new Vector2Int(Random.Range(0, resolution.x), Random.Range(0, resolution.y));
            mCentroids[i] = new Vector3Int((int)(poissonList[i].x * resolution.x / 2 + resolution.x / 2), (int)(poissonList[i].y * resolution.y / 2 + resolution.y / 2), 0);
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
        SaveCentralPoints();
        CenterPoints();
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

    private int GetClosestCentroidIndex(Vector2Int pixelPos, Vector3Int[] centroids)
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