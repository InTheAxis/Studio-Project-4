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
    public Entity decal0;
    public Entity decal1;
    public Entity decal2;
    public Entity decal3;
    public Entity decal4;
    public Entity decal5;
    public Entity decal6;
    public Entity decal7;
    public Entity decal8;
    public Entity decal9;
    public Entity decal10;
    public Entity decal11;
    public Entity decal12;
    public Entity decal13;
    public Entity decal14;
    public Entity decal15;
    public Entity decal16;
    //public Entity rockC;
}