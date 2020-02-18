using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CityGenerator : MonoBehaviour
{
    public GameObject buildingRoot;
    public List<Generator> generators;

    public float scale = 1;

    [Range(0, 100)]
    public int density = 5;

    [Range(0, 1)]
    public float buffer = 0.1f;

    private PoissonGenerator poisson = new PoissonGenerator();

    public GameObject buildingRef;
    // public bool serverInstantiate = false;

    public bool generateOnStart = false;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient && generateOnStart)
            Generate();
    }

    private void OnValidate()
    {
        foreach (Generator generator in generators)
        {
            generator.SetScale(scale);
        }
    }

    public void Clear()
    {
        while (buildingRoot.transform.childCount > 0)
        {
            DestroyImmediate(buildingRoot.transform.GetChild(0).gameObject);
        }
        foreach (Generator generator in generators)
        {
            generator.Clear();
        }
    }

    public void Generate()
    {
        Clear();

        foreach (Generator generator in generators)
        {
            generator.Generate();
        }

        // CreateBuildings();
    }

    private void OnDrawGizmos()
    {
        foreach (Transform trans in buildingRoot.transform)
        {
            Gizmos.DrawWireSphere(trans.position, buffer * scale * 50);
        }
    }

    private void CreateBuildings()
    {
        poisson.Set(density, buffer);
        poisson.Generate();
        poisson.SetToXZ();
        poisson.Scale(scale * 100 / 2);
        foreach (Vector3 pos in poisson.GetPoints())
        {
            GameObject building = InstantiateHandler.mInstantiate(buildingRef, buildingRoot.transform);
            building.transform.position = pos;
            building.AddComponent<BuildingCollisionScript>();
            building.GetComponent<ProceduralBuilding>().GenerateRandom();
            building.transform.rotation = Quaternion.Euler(0, Random.Range(0, 359), 0);
        }
    }
}