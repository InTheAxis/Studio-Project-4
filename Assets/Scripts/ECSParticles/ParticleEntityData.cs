using Unity.Entities;
using Unity.Mathematics;

public struct ParticleAliveTag : IComponentData { }
public struct ParticleEntityData : IComponentData
{
    //obj settings
    public float3 emitSource;
    public float3 emitRandom;

    public float3 rotRandom;
    public float3 startRot;
    public bool rotOverTime;   
    public float3 endRot;
    
    public float scaleRandom;
    public float startScale;
    public bool scaleOverTime;   
    public float endScale;
    
    public float3 velRandom;
    public float startSpeed;
    public bool velOverTime;
    public float3 endVel;

    public float3 gravityRandom;
    public float3 startGravity;
    public bool gravityOverTime;
    public float3 endGravity;

    public float lifeTimeRandom;
    public float lifeTime;

    //internal vals for system to read
    public float3 vel;
    public float3 rot;
    public float3 gravity;
    public float3 _startRot;
    public float _startScale;
    public float3 _startVel;
    public float3 _endRot;
    public float _endScale;
    public float3 _startGravity;
    public float3 _endGravity;
    public float3 _endVel;
    public float life;

    public static ParticleEntityData Create(ParticleEntityDataObj obj)
    {
        ParticleEntityData data = new ParticleEntityData();

        data.emitSource = obj.emitSource;
        data.emitRandom = obj.emitRandom;

        data.rotRandom = obj.rotRandom;
        data.startRot = obj.startRot;
        data.rotOverTime = obj.rotOverTime;
        data.endRot = obj.endRot;

        data.scaleRandom = obj.scaleRandom;
        data.startScale = obj.startScale;
        data.scaleOverTime = obj.scaleOverTime;
        data.endScale = obj.endScale;

        data.velRandom = obj.velRandom;
        data.startSpeed = obj.startSpeed;
        data.velOverTime = obj.velOverTime;
        data.endVel = obj.endVel;

        data.gravityRandom = obj.gravityRandom;
        data.startGravity = obj.startGravity;
        data.gravityOverTime = obj.gravityOverTime;
        data.endGravity = obj.endGravity;

        data.lifeTimeRandom = obj.lifeTimeRandom;
        data.lifeTime = obj.lifeTime;


        return data;
    }
}
