using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[CreateAssetMenu]
public class ParticleEntityDataObj : ScriptableObject
{
    public Mesh mesh;
    public Material material;

    [Header("Position")]
    public Vector3 emitAreaMin;
    public Vector3 emitAreaMax;

    [Header("Rotation")]
    public Vector3 startRotMin;
    public bool rotOverTime;
    public Vector3 endRotMin;    

    [Header("Scale")]
    public float startScaleMin;
    public bool scaleOverTime;
    public float endScaleMin;

    [Header("Velocity")]
    public Vector3 startVelMin;
    public bool velOverTime;
    public Vector3 endVelMin;

    [Header("Gravity")]
    public Vector3 startGravity;
    public bool gravityOverTime;
    public Vector3 endGravity;

    [Header("Lifetime")]
    public float lifeTime;
};


public struct ParticleAliveTag : IComponentData { }
public struct ParticleEntityData : IComponentData
{
    //obj settings
    public float3 emitAreaMax;
    public float3 emitAreaMin;

    public float3 startRotMin;
    public bool rotOverTime;   
    public float3 endRotMin;
    
    public float startScaleMin;
    public bool scaleOverTime;   
    public float endScaleMin;
    
    public float3 startVelMin;
    public bool velOverTime;
    public float3 endVelMin;

    public float3 startGravity;
    public bool gravityOverTime;
    public float3 endGravity;


    public float lifeTime;

    //internal vals for system to read
    public float3 vel;
    public float3 rot;
    public float3 gravity;
    public float3 startRot;
    public float startScale;
    public float3 startVel;
    public float3 endRot;
    public float endScale;
    public float3 endVel;
    public float life;

    public static ParticleEntityData Create(ParticleEntityDataObj obj)
    {
        ParticleEntityData data = new ParticleEntityData();

        data.emitAreaMax = obj.emitAreaMax;
        data.emitAreaMin = obj.emitAreaMin;
        
        data.startRotMin = obj.startRotMin;
        data.endRotMin = obj.endRotMin;
        
        data.startScaleMin = obj.startScaleMin;
        data.endScaleMin = obj.endScaleMin;
        
        data.startVelMin = obj.startVelMin;
        data.endVelMin = obj.endVelMin;

        data.startGravity = obj.startGravity;
        data.endGravity = obj.endGravity;

        data.lifeTime = obj.lifeTime;

        data.rotOverTime = obj.rotOverTime;
        data.scaleOverTime = obj.scaleOverTime;
        data.velOverTime = obj.velOverTime;
        data.gravityOverTime = obj.gravityOverTime;

        return data;
    }
}
