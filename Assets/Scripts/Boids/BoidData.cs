using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct BoidData : IComponentData
{
    public BoidData(Vector3 vel)
    {
        this.vel = vel;
    }

    public Vector3 vel;
}