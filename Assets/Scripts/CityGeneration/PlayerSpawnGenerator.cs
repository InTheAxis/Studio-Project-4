using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnGenerator : Generator
{
    private bool generated = false;

    [Range(0, 1)]
    [SerializeField]
    private float hunterBuffer;

    [Range(0, 1)]
    [SerializeField]
    private float playerBuffer;

    public List<Vector3> playerSpawnPos { get; private set; }

    public Vector3 hunterSpawnPos { get; private set; }

    private PoissonGenerator poisson = new PoissonGenerator();

    [SerializeField]
    private TowerGenerator towerGenerator;

    [Range(0, 1)]
    [SerializeField]
    private float towerBuffer;

    [Range(0, 1)]
    [SerializeField]
    private float centerBuffer;

    public override void Clear()
    {
        generated = false;
    }

    public override void Generate()
    {
        Debug.Log("Generating Spawn");
        generated = true;
        playerSpawnPos.Clear();
        hunterSpawnPos = Vector3.zero;
        poisson.ClearInjected();
        poisson.Inject(new PoissonPoint(Vector3.zero, centerBuffer));
        foreach (PoissonPoint point in towerGenerator.GetPoisson().GetPoints(1))
        {
            poisson.Inject(new PoissonPoint(point.pos / scale, towerBuffer));
        }
        bool successA = poisson.Generate(4, playerBuffer, 200);
        if (!successA || poisson.GetPoints().Count < 9)
            Debug.LogError("Failed to generate player spawn location");
        // hunter
        PoissonGenerator hunterPoisson = new PoissonGenerator();
        hunterPoisson.Inject(new PoissonPoint(Vector3.zero, centerBuffer));
        foreach (PoissonPoint point in towerGenerator.GetPoisson().GetPoints(1))
        {
            hunterPoisson.Inject(new PoissonPoint(point.pos / scale, towerBuffer));
        }
        foreach (PoissonPoint point in poisson.GetPoints(towerGenerator.GetPoisson().GetPoints().Count))
        {
            playerSpawnPos.Add(point.pos * scale);
            hunterPoisson.Inject(new PoissonPoint(point.pos, point.radius));
        }
        if (playerSpawnPos.Count < 4)
            Debug.LogError("Failed to generate player spawn locationB");
        bool success = hunterPoisson.Generate(1, hunterBuffer, 200);
        if (!success)
            Debug.LogError("Failed to generate hunter spawn location");
        hunterSpawnPos = hunterPoisson.GetPoints()[hunterPoisson.GetPoints().Count - 1].pos * scale;
    }

    private void OnDrawGizmos()
    {
        if (!gizmosEnabled)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Vector3.zero, centerBuffer * scale);
        Gizmos.color = Color.magenta;
        foreach (Vector3 pos in playerSpawnPos)
            Gizmos.DrawWireSphere(pos, playerBuffer * scale);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(hunterSpawnPos, hunterBuffer * scale);
        Gizmos.color = Color.red;
        foreach (PoissonPoint point in towerGenerator.GetPoisson().GetPoints(1))
            Gizmos.DrawWireSphere(point.pos, towerBuffer * scale);
    }
}