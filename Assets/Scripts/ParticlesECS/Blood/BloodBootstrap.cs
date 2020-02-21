using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using Unity.Mathematics;

public struct BloodTag : IComponentData { }
public class BloodBootstrap : ParticleBootstrap
{
    public static BloodBootstrap Instance;
    private void Start()
    {
        Instance = this;

        Init(typeof(BloodTag));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            if (isEmitting) StopEmit();
            else            Emit();
    }
}