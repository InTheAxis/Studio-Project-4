using Unity.Entities;
using Unity.Mathematics;

public struct ParticleAliveTag : IComponentData { }
public struct ParticleEntityData : IComponentData
{
    //obj settings

    public float3 startRot;
    public float3 rotRandom;
    public bool rotOverTime;   
    public float3 endRot;
    
    public float startScale;
    public float scaleRandom;
    public bool scaleOverTime;   
    public float endScale;
    
    public float startSpeed;
    public float3 velRandom;
    public bool velOverTime;
    public float3 endVel;

    public float3 startGravity;
    public float3 gravityRandom;
    public bool gravTowardsSource;
    public bool gravityOverTime;
    public float3 endGravity;

    public float lifeTime;
    public float lifeTimeRandom;

    //internal vals for system to read
    public float3 emitSource; //not really used currently
    public float3 emitDir;
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

        data.emitSource = float3.zero;
        data.emitDir = new float3(0, 1, 0);

        data.startRot = obj.startRot;
        data.rotRandom = obj.rotRandom;
        data.rotOverTime = obj.rotOverTime;
        data.endRot = obj.endRot;

        data.startScale = obj.startScale;
        data.scaleRandom = obj.scaleRandom;
        data.scaleOverTime = obj.scaleOverTime;
        data.endScale = obj.endScale;

        data.startSpeed = obj.startSpeed;
        data.velRandom = obj.velRandom;
        data.velOverTime = obj.velOverTime;
        data.endVel = obj.endVel;

        data.startGravity = obj.startGravity;
        data.gravityRandom = obj.gravityRandom;
        data.gravTowardsSource = obj.gravTowardsSource;
        data.gravityOverTime = obj.gravityOverTime;
        data.endGravity = obj.endGravity;

        data.lifeTime = obj.lifeTime;
        data.lifeTimeRandom = obj.lifeTimeRandom;

        return data;
    }
}
