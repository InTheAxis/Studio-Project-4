using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[Serializable]
[GenerateAuthoringComponent]
public struct BoidManagerData : IComponentData
{
    // Add fields to your component here. Remember that:
    //
    // * A component itself is for storing data and doesn't 'do' anything.
    //
    // * To act on the data, you will need a System.
    //
    // * Data in a component must be blittable, which means a component can
    //   only contain fields which are primitive types or other blittable
    //   structs; they cannot contain references to classes.
    //
    // * You should focus on the data structure that makes the most sense
    //   for runtime use here. Authoring Components will be used for
    //   authoring the data in the Editor.
    [Range(0, 1.0f)]
    public float speed;

    [Range(0, 0.1f)]
    public float alignRate;

    [Range(0, 0.1f)]
    public float coheseRate;

    [Range(0, 0.1f)]
    public float separateRate;

    [Range(0, 1.0f)]
    public float avoidGroundRate;

    public float viewRadius;
    public float separateRadius;

    public bool align;
    public bool cohese;
    public bool separate;

    public float range;
    public float avoidGroundHeight;
    public float avoidCeilingHeight;
    public float minHeight;
    public float maxHeight;
}