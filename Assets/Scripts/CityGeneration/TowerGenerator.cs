using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGenerator : Generator
{
    public PoissonGenerator poisson = new PoissonGenerator();
    public GameObject towerRef;

    [SerializeField]
    [Min(0)]
    private float centerBuffer = 10.1f;

    [Range(0, 1)]
    private float offset = 0.55f;

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
        poisson.Inject(new PoissonPoint(Vector3.zero, centerBuffer / scale));
        poisson.GenerateDensity(4, offset);
        poisson.Scale(scale);
        CreateTowers();
    }

    private void CreateTowers()
    {
        foreach (PoissonPoint point in poisson.GetPoints(1))
        {
            InstantiateHandler.mInstantiate(towerRef, point.pos, Quaternion.identity, transform, "Interactable");
        }
    }

    private void OnDrawGizmos()
    {
        if (!gizmosEnabled)
            return;
        // Gizmos.DrawWireSphere(Vector3.zero, scale);
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(Vector3.zero, 0.2f * scale);
        Gizmos.color = Color.green;
        foreach (PoissonPoint poissonPoint in poisson.GetPoints())
        {
            Gizmos.DrawWireSphere(poissonPoint.pos, poissonPoint.radius);
        }
    }
}