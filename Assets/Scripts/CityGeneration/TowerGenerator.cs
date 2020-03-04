using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGenerator : Generator
{
    public PoissonGenerator poisson = new PoissonGenerator();
    public GameObject[] towerRef;
    public List<GameObject> towers { get; private set; } = new List<GameObject>();

    [SerializeField]
    [Min(0)]
    private float centerBuffer = 10.1f;

    [SerializeField]
    public float towerRange = 70;

    [Range(0, 1)]
    private float offset = 0.55f;

    public PoissonGenerator GetPoisson()
    {
        return poisson;
    }

    public override void Clear()
    {
        towers.Clear();
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
        int counter = 0;
        foreach (PoissonPoint point in poisson.GetPoints(1))
        {
            Vector3 pos = point.pos;
            pos.y += 0.01f;
            towers.Add(InstantiateHandler.mInstantiate(towerRef[counter], pos, Quaternion.identity, transform));
            counter++;
        }
        //foreach (GameObject tower in towers)
        //{
        //    if (!tower)
        //        continue;
        //    Collider[] coll = Physics.OverlapSphere(tower.transform.position, towerRange * 1f);
        //    foreach (Collider col in coll)
        //    {
        //        if (col.gameObject.layer == LayerMask.NameToLayer("Road"))
        //            DestroyImmediate(col.gameObject);
        //    }
        //}
    }

    private void OnDrawGizmos()
    {
        if (!gizmosEnabled)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, centerBuffer);
        //Gizmos.color = Color.green;
        //foreach (GameObject tower in towers)
        //{
        //    Gizmos.DrawWireSphere(tower.transform.position, buff);
        //}
        Gizmos.color = Color.red;
        foreach (GameObject tower in towers)
        {
            Gizmos.DrawWireSphere(tower.transform.position, towerRange);
        }
    }
}