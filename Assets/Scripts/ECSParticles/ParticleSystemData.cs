using Unity.Entities;
using Unity.Mathematics;

public struct ParticleEmitTag : IComponentData { } //to label whether to emit
public struct ParticleSystemData : IComponentData 
{
    public int numJobBatch;

    public int numPerUpdate;
    public float rate;

    public bool loop;
    public float duration;
    public float delay;

    public float3 pos;
    public float3 dir;

    public static ParticleSystemData Create(ParticleSystemDataObj obj)
    {
        ParticleSystemData data = new ParticleSystemData();

        data.numJobBatch = obj.numJobBatch;

        data.numPerUpdate = obj.numPerUpdate;
        data.rate = obj.rate;

        data.loop = obj.loop;
        data.duration = obj.duration;
        data.delay = obj.delay;

        return data;
    }
}

