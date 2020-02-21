using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[CreateAssetMenu]
public class ParticleEntityDataObj : ScriptableObject
{
    public Mesh mesh;
    public Material material;

    public Vector3 emitAreaMax;
    public Vector3 emitAreaMin;

    public Vector3 startRot;
    public Vector3 endRot;

    public float startScale;
    public float endScale;

    public Vector3 startVel;
    public Vector3 endVel;

    public float lifeTime;
};


public struct ParticleAliveTag : IComponentData { }
public struct ParticleEntityData : IComponentData
{
    //obj settings
    public float3 emitAreaMax;
    public float3 emitAreaMin;
    public float3 startRot;
    public float3 endRot;
    public float startScale;
    public float endScale;
    public float3 startVel;
    public float3 endVel;

    public float lifeTime;

    //internal vals for system to read
    public float3 vel;
    public float3 rot;
    public float life;

    public static ParticleEntityData Create(ParticleEntityDataObj obj)
    {
        ParticleEntityData data = new ParticleEntityData();

        data.emitAreaMax = obj.emitAreaMax;
        data.emitAreaMin = obj.emitAreaMin;
        data.startRot = obj.startRot;
        data.endRot = obj.endRot;
        data.startScale = obj.startScale;
        data.endScale = obj.endScale;
        data.startVel = obj.startVel;
        data.endVel = obj.endVel;
        data.lifeTime = obj.lifeTime;

        return data;
    }
}
