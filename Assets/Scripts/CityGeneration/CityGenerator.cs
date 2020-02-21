using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CityGenerator : MonoBehaviour
{
    //public List<Generator> generators;
    public List<GeneratorData> generators;

    public float scale = 1;

    // public bool serverInstantiate = false;

    public bool generateOnStart = false;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient && generateOnStart)
            Generate();
    }

    private void OnValidate()
    {
        foreach (GeneratorData generator in generators)
        {
            generator.generator.SetScale(scale);
        }
    }

    public void Clear()
    {
        foreach (GeneratorData generator in generators)
        {
            generator.generator.Clear();
        }
    }

    public void Generate()
    {
        Clear();

        foreach (GeneratorData generator in generators)
        {
            if (generator.enabled)
                generator.generator.Generate();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Vector3.zero, scale);
    }
}