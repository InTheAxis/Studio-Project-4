using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGenerator : Generator
{
    public PoissonGenerator poisson = new PoissonGenerator();
    public GameObject towerRef;

    public PoissonGenerator GetPoisson()
    {
        return poisson;
    }
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
        poisson.Inject(new PoissonPoint(Vector3.zero, 0.1f));
        poisson.GenerateDensity(4, 0.6f);
        poisson.Scale(scale);
        CreateTowers();
    }

    private void CreateTowers()
    {
        foreach (PoissonPoint point in poisson.GetPoints(1))
        {
            InstantiateHandler.mInstantiate(towerRef, point.pos, transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Vector3.zero, scale);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, 0.2f * scale);
        Gizmos.color = Color.green;
        foreach (PoissonPoint poissonPoint in poisson.GetPoints())
        {
            Gizmos.DrawWireSphere(poissonPoint.pos, poissonPoint.radius);
        }
    }
}