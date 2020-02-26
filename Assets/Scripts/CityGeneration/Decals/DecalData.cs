using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct DecalData : IComponentData
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
    public int numRock;

    public int density;
    public int range;
    public Entity rock0;
    public Entity rock1;
    public Entity rockC;
    public Entity rockD;
    public Entity rockE;
    public Entity rockF;
    public Entity rockG;
    public Entity decal7;
    public Entity decal8;
    public Entity decal9;
    public Entity decal10;
    public Entity decal11;
    public Entity decal12;
    public Entity decal13;
    public Entity decal14;
    //public Entity rockC;
}