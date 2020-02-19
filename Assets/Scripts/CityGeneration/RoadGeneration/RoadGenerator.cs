using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : Generator
{
    //
    public SpriteRenderer sprite;

    //
    public int resolution = 100;

    public bool cleanUp;

    //
    public GameObject RoadRef;

    [Range(0, 200)]
    public int density = 10;

    public float length = 140;

    public SubdivisionValue sub;
    public SubdivisionValue minor;

    private Voronoi voronoi = new Voronoi();
    private List<RoadPath> roadInnerPaths = new List<RoadPath>();
    private List<RoadPath> roadOuterPaths = new List<RoadPath>();
    private List<RoadPath> roadSubPaths = new List<RoadPath>();
    private List<RoadPath> roadMinorPaths = new List<RoadPath>();

    private struct IntersectionNode
    {
        public Vector3 pos;
        public float angle;
    }

    [System.Serializable]
    public struct SubdivisionValue
    {
        public float minOffset;
        public float minDist;
        public float maxDist;
        public float minLength;
        public float maxLength;
    }

    public struct RoadPath
    {
        public RoadPath(Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;
            this.dir = (end - start).normalized;
            // this.length = (end - start).magnitude;
        }

        public Vector3 start;
        public Vector3 end;
        public Vector3 dir;

        public float Length()
        {
            return (end - start).magnitude;
        }
    }

    public List<RoadPath> GetRoadInnerPaths()
    {
        return roadInnerPaths;
    }

    public List<RoadPath> GetRoadOuterPaths()
    {
        return roadOuterPaths;
    }

    public Voronoi GetVoronoi()
    {
        return voronoi;
    }

    private void OnDrawGizmos()
    {
        if (!voronoi.IsGenerated())
            return;
        Gizmos.color = Color.cyan;
        foreach (RoadPath path in roadInnerPaths)
        {
            Gizmos.DrawLine(path.start, path.end);
        }
        Gizmos.color = Color.blue;
        foreach (RoadPath path in roadOuterPaths)
        {
            Gizmos.DrawLine(path.start, path.end);
        }
        Gizmos.color = Color.red;
        foreach (RoadPath path in roadSubPaths)
        {
            Gizmos.DrawLine(path.start, path.end);
        }
        Gizmos.color = Color.green;
        foreach (RoadPath path in roadMinorPaths)
        {
            Gizmos.DrawLine(path.start, path.end);
        }
    }

    private void CleanUpOuterPaths()
    {
        for (int i = 0; i < roadOuterPaths.Count; ++i)
        {
            RoadPath path = roadOuterPaths[i];
            path.end += roadOuterPaths[i].dir * (length + Mathf.Sqrt(path.Length()));
            roadOuterPaths[i] = path;
        }
    }

    public override void Generate()
    {
        Clear();
        GenerateMainRoads();
        CleanUpOuterPaths();
        GenerateSubPaths();
        IteratePath();
        if (cleanUp)
            RemoveDeadEnds();
        CreateRoads();
    }

    public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        closestPointLine1 = Vector3.zero;
        closestPointLine2 = Vector3.zero;

        float a = Vector3.Dot(lineVec1, lineVec1);
        float b = Vector3.Dot(lineVec1, lineVec2);
        float e = Vector3.Dot(lineVec2, lineVec2);

        float d = a * e - b * b;

        //lines are not parallel
        if (d != 0.0f)
        {
            Vector3 r = linePoint1 - linePoint2;
            float c = Vector3.Dot(lineVec1, r);
            float f = Vector3.Dot(lineVec2, r);

            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;

            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;

            return true;
        }
        else
        {
            return false;
        }
    }

    private void RemoveDeadEnds()
    {
        // shorten sub roads to outer roads
        for (int i = 0; i < roadSubPaths.Count; ++i)
        {
            RoadPath path = roadSubPaths[i];
            Vector3 line1Point = path.start;
            Vector3 line1Vec = path.dir;
            //int index = -1;
            for (int j = 0; j < roadOuterPaths.Count; ++j)
            {
                RoadPath path2 = roadOuterPaths[j];
                //if (path.Equals(path2))
                //    continue;
                Vector3 line2Point = path2.start;
                Vector3 line2Vec = path2.dir;
                Vector3 intersection;
                if (LineLineIntersection(out intersection, line1Point, line1Vec, line2Point, line2Vec))
                {
                    if (Vector3.Distance(intersection, (path.start + path.end) / 2) < 0.5)
                        continue;
                    Vector3 dir = (intersection - path.start).normalized;
                    float distIA = Vector3.Distance(intersection, path.start);
                    float distIB = Vector3.Distance(intersection, path2.start);
                    float lengthA = path.Length();
                    float lengthB = path2.Length();
                    if (distIA < lengthA && distIB < lengthB)
                    {
                        if (Vector3.Distance(intersection, path.start) < Vector3.Distance(intersection, path.end))
                        {
                            path.start = intersection;
                        }
                        else
                            path.end = intersection;
                        roadSubPaths[i] = path;
                    }
                }
            }
        }
        // shorten sub roads to sub roads
        for (int i = 0; i < roadSubPaths.Count; ++i)
        {
            RoadPath path = roadSubPaths[i];
            Vector3 line1Point = path.start;
            Vector3 line1Vec = path.dir;
            //int index = -1;
            for (int j = i + 1; j < roadSubPaths.Count; ++j)
            {
                RoadPath path2 = roadSubPaths[j];
                //if (path.Equals(path2))
                //    continue;
                Vector3 line2Point = path2.start;
                Vector3 line2Vec = path2.dir;
                Vector3 intersection;
                if (LineLineIntersection(out intersection, line1Point, line1Vec, line2Point, line2Vec))
                {
                    if (Vector3.Distance(intersection, (path.start + path.end) / 2) < 0.5)
                        continue;
                    Vector3 dir = (intersection - path.start).normalized;
                    float distIA = Vector3.Distance(intersection, path.start);
                    float distIB = Vector3.Distance(intersection, path2.start);
                    float lengthA = path.Length();
                    float lengthB = path2.Length();
                    if (distIA < lengthA && distIB < lengthB)
                    {
                        if (Vector3.Distance(intersection, path.start) < Vector3.Distance(intersection, path.end))
                        {
                            path.start = intersection;
                        }
                        else
                            path.end = intersection;
                        roadSubPaths[i] = path;
                    }
                }
            }
        }
        // shorten minor roads to outer roads
        for (int i = 0; i < roadMinorPaths.Count; ++i)
        {
            RoadPath path = roadMinorPaths[i];
            Vector3 line1Point = path.start;
            Vector3 line1Vec = path.dir;
            bool hasIntersection = false;
            //int index = -1;
            for (int j = 0; j < roadOuterPaths.Count; ++j)
            {
                RoadPath path2 = roadOuterPaths[j];
                Vector3 line2Point = path2.start;
                Vector3 line2Vec = path2.dir;
                Vector3 intersection;
                if (LineLineIntersection(out intersection, line1Point, line1Vec, line2Point, line2Vec))
                {
                    //if (Vector3.Distance(intersection, (path.start + path.end) / 2) < 0.5)
                    //    continue;
                    Vector3 dir = (intersection - path.start).normalized;
                    float distIA = Vector3.Distance(intersection, path.start);
                    float distIB = Vector3.Distance(intersection, path2.start);
                    float lengthA = path.Length();
                    float lengthB = path2.Length();
                    if (distIA < lengthA && distIB < lengthB)
                    {
                        hasIntersection = true;
                        if (Vector3.Distance(intersection, path.start) < Vector3.Distance(intersection, path.end))
                        {
                            path.start = intersection;
                        }
                        else
                            path.end = intersection;
                        roadMinorPaths[i] = path;

                        break;
                    }
                }
            }
            if (!hasIntersection)
            {
                roadMinorPaths.RemoveAt(i);
                --i;
            }
        }
        // shorten minor roads
        for (int i = 0; i < roadMinorPaths.Count; ++i)
        {
            RoadPath path = roadMinorPaths[i];
            Vector3 line1Point = path.start;
            Vector3 line1Vec = path.dir;
            bool hasIntersection = false;
            //int index = -1;
            for (int j = 0; j < roadSubPaths.Count; ++j)
            {
                RoadPath path2 = roadSubPaths[j];
                Vector3 line2Point = path2.start;
                Vector3 line2Vec = path2.dir;
                Vector3 intersection;
                if (LineLineIntersection(out intersection, line1Point, line1Vec, line2Point, line2Vec))
                {
                    //if (Vector3.Distance(intersection, (path.start + path.end) / 2) < 0.5)
                    //    continue;
                    Vector3 dir = (intersection - path.start).normalized;
                    float distIA = Vector3.Distance(intersection, path.start);
                    float distIB = Vector3.Distance(intersection, path2.start);
                    float lengthA = path.Length();
                    float lengthB = path2.Length();
                    if (distIA < lengthA && distIB < lengthB)
                    {
                        hasIntersection = true;
                        if (Vector3.Distance(intersection, path.start) < Vector3.Distance(intersection, path.end))
                        {
                            path.start = intersection;
                        }
                        else
                            path.end = intersection;
                        roadMinorPaths[i] = path;

                        break;
                    }
                }
            }
            if (!hasIntersection)
            {
                roadMinorPaths.RemoveAt(i);
                --i;
            }
        }

        // remove deadend minor roads
        for (int i = 0; i < roadMinorPaths.Count; ++i)
        {
            RoadPath path = roadMinorPaths[i];
            Vector3 line1Point = path.start;
            Vector3 line1Vec = path.dir;
            bool hasIntersection = false;
            //int index = -1;
            for (int j = 0; j < roadMinorPaths.Count; ++j)
            {
                RoadPath path2 = roadMinorPaths[j];
                //if (path.Equals(path2))
                //    continue;
                Vector3 line2Point = path2.start;
                Vector3 line2Vec = path2.dir;
                Vector3 intersection;
                if (LineLineIntersection(out intersection, line1Point, line1Vec, line2Point, line2Vec))
                {
                    Vector3 dir = (intersection - path.start).normalized;
                    float distIA = Vector3.Distance(intersection, path.start);
                    float distIB = Vector3.Distance(intersection, path2.start);
                    float lengthA = path.Length();
                    float lengthB = path2.Length();
                    if (distIA < lengthA && distIB < lengthB)
                    {
                        if (Vector3.Distance(intersection, (path.end)) < 0.5f || Vector3.Distance(intersection, (path.start)) < 0.5f)
                        {
                            if (Vector3.Distance(intersection, path.start) < Vector3.Distance(intersection, path.end))
                            {
                                path.start = intersection;
                            }
                            else
                                path.end = intersection;
                            roadMinorPaths[i] = path;
                        }
                        hasIntersection = true;
                        break;
                    }
                }
            }
            if (!hasIntersection)
            {
                roadMinorPaths.RemoveAt(i);
                --i;
            }
        }
    }

    private List<GameObject> roads = new List<GameObject>();

    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parrallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }

    public override void Clear()
    {
        roadOuterPaths.Clear();
        roadInnerPaths.Clear();
        roadSubPaths.Clear();
        roadMinorPaths.Clear();

        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public void CreateRoads()
    {
        foreach (GameObject gameObject in roads)
        {
            DestroyImmediate(gameObject);
        }
        roads.Clear();
        float length = 0.1f;
        foreach (RoadPath path in roadInnerPaths)
        {
            //Gizmos.DrawLine(path.start, path.end);
            Quaternion rot = Quaternion.LookRotation(path.dir, Vector3.up);
            GameObject road = InstantiateHandler.mInstantiate(RoadRef, (path.start + path.end) / 2, rot, transform, "Environment") as GameObject;
            road.transform.localScale = new Vector3(2, 1, path.Length() * length);
            roads.Add(road);
        }
        foreach (RoadPath path in roadOuterPaths)
        {
            //Gizmos.DrawLine(path.start, path.end);
            Quaternion rot = Quaternion.LookRotation(path.dir, Vector3.up);
            GameObject road = InstantiateHandler.mInstantiate(RoadRef, (path.start + path.end) / 2, rot, transform, "Environment") as GameObject;
            road.transform.localScale = new Vector3(2, 1, path.Length() * length);
            roads.Add(road);
        }
        foreach (RoadPath path in roadSubPaths)
        {
            //Gizmos.DrawLine(path.start, path.end);
            Quaternion rot = Quaternion.LookRotation(path.dir, Vector3.up);
            GameObject road = InstantiateHandler.mInstantiate(RoadRef, (path.start + path.end) / 2, rot, transform, "Environment");
            road.transform.localScale = new Vector3(1, 1, path.Length() * length);
            roads.Add(road);
        }
        foreach (RoadPath path in roadMinorPaths)
        {
            //Gizmos.DrawLine(path.start, path.end);
            Quaternion rot = Quaternion.LookRotation(path.dir, Vector3.up);
            GameObject road = InstantiateHandler.mInstantiate(RoadRef, (path.start + path.end) / 2, rot, transform, "Environment");
            road.transform.localScale = new Vector3(1, 1, path.Length() * length);
            roads.Add(road);
        }
    }

    private void GenerateSubPaths()
    {
        foreach (RoadPath path in roadOuterPaths)
        {
            float pathLength = Vector3.Distance(path.start, path.end);
            float currentLength = 0;
            while (currentLength < pathLength)
            {
                float distance = Random.Range(sub.minDist, sub.maxDist) * scale;
                currentLength += distance;
                if (currentLength > pathLength)
                    break;
                if (currentLength < sub.minOffset)
                    break;
                Vector3 pos = path.start + currentLength * path.dir;
                Vector3 perpen = Vector3.Cross(path.dir, Vector3.up).normalized;
                float subpathLength = Random.Range(sub.minLength, sub.maxLength) * scale;
                roadSubPaths.Add(new RoadPath(pos - perpen * subpathLength / 2, pos + perpen * subpathLength / 2));
            }
        }
    }

    private void IteratePath()
    {
        SubdivisionValue subdivision = minor;
        foreach (RoadPath path in roadSubPaths)
        {
            float pathLength = Vector3.Distance(path.start, path.end);
            float currentLength = 0;
            while (currentLength < pathLength)
            {
                float distance = Random.Range(subdivision.minDist, subdivision.maxDist) * scale;
                currentLength += distance;
                if (currentLength > pathLength)
                    break;
                if (currentLength < subdivision.minOffset)
                    break;
                Vector3 pos = path.start + currentLength * path.dir;
                Vector3 perpen = Vector3.Cross(path.dir, Vector3.up).normalized;
                float subpathLength = Random.Range(subdivision.minLength, subdivision.maxLength) * scale;
                roadMinorPaths.Add(new RoadPath(pos - perpen * subpathLength / 2, pos + perpen * subpathLength / 2));
            }
        }
    }

    public void GenerateMainRoads()
    {
        // generate voronoi
        voronoi.Generate(new Vector2Int(resolution, resolution), density);
        if (sprite)
        {
            sprite.sprite = Sprite.Create(voronoi.GetVoronoiTexture(), new Rect(0, 0, resolution, resolution), Vector2.one * 0.5f);
            sprite.GetComponent<Transform>().localScale = (Vector3.one * 100 * scale);
        }
        //
        // scale voronoi
        voronoi.Scale(scale);
        //
        // Path central road
        Vector3 center = new Vector3();
        foreach (VoronoiVertice vertice in voronoi.GetCentral())
        {
            center += vertice.pos;
        }
        center /= voronoi.GetCentral().Count;
        List<IntersectionNode> intersectionNodes = new List<IntersectionNode>();
        foreach (VoronoiVertice vertice in voronoi.GetCentral())
        {
            Vector3 verticePos = vertice.pos;
            float angle = Vector3.SignedAngle(Vector3.forward, verticePos - center, Vector3.up);
            if (angle < 0)
                angle += 360;
            IntersectionNode node = new IntersectionNode();
            node.pos = vertice.pos;
            node.angle = angle;
            intersectionNodes.Add(node);
        }
        intersectionNodes.Sort((p1, p2) => p1.angle.CompareTo(p2.angle));
        for (int i = 0; i < intersectionNodes.Count - 1; ++i)
        {
            Vector3 point = intersectionNodes[i].pos;
            Vector3 point1 = intersectionNodes[i + 1].pos;
            roadInnerPaths.Add(new RoadPath(point, point1));
        }
        Vector3 pointa = intersectionNodes[0].pos;
        Vector3 pointb = intersectionNodes[intersectionNodes.Count - 1].pos;
        roadInnerPaths.Add(new RoadPath(pointa, pointb));
        //
        // outer paths
        foreach (VoronoiVertice vertice in voronoi.GetCentral())
        {
            foreach (VoronoiVertice other in voronoi.GetVoronoiPoints())
            {
                if (voronoi.GetCentral().Contains(other))
                    continue;
                int counter = 0;
                for (int i = 0; i < other.centroidIndexes.Count; ++i)
                {
                    if (vertice.centroidIndexes.Contains(other.centroidIndexes[i]))
                        ++counter;
                }
                if (counter > 1)
                {
                    roadOuterPaths.Add(new RoadPath(vertice.pos, other.pos));
                    break;
                }
            }
        }
        //
    }
}