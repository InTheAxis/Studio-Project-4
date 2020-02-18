using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshData
{
    //public Mesh mesh;
    public GameObject mesh;

    [Range(0, 100)]
    public int weight;

    public float chance;
}