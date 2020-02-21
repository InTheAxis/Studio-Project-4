using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[CreateAssetMenu]
public class ParticleEntityDataObj : ScriptableObject
{
    public float scale;
    public Mesh mesh;
    public Material material;


    public Vector3 velMin;
    public Vector3 velMax;
    public float lifeTime;
};


public struct ParticleAliveTag : IComponentData { }
public struct ParticleEntityData : IComponentData
{
    //obj settings
    public float3 velMin;
    public float3 velMax;
    public float lifeTime;

    //internal vals for system to read
    public float3 vel;
    public float life;

    public static ParticleEntityData Create(ParticleEntityDataObj obj)
    {
        ParticleEntityData data = new ParticleEntityData();

        data.velMin = obj.velMin;
        data.velMax = obj.velMax;
        data.lifeTime = obj.lifeTime;

        return data;
    }
}
