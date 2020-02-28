using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct BoidSpawnerData : IComponentData
{
    public Entity boidRef;
    public int numSpawn;
    public int range;
    public float buffer;
}