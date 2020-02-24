using Unity.Entities;
using Unity.Mathematics;

public struct ParticleEmitTag : IComponentData { } //to label whether to emit
public struct ParticleSystemData : IComponentData 
{
    public int numJobBatch;

    public float3 emitPosRandom;
    public int numPerUpdate;
    public float rate;

    public bool loop;
    public float duration;
    public float delay;

    //to be set by bootstrap
    public float3 pos;
    public float3 dir;

    //internal values for direction type
    public int emitDirType;

    public static ParticleSystemData Create(ParticleSystemDataObj obj)
    {
        ParticleSystemData data = new ParticleSystemData();

        data.numJobBatch = obj.numJobBatch;

        data.emitPosRandom = obj.emitPosRandom;
        data.emitDirType = (int)obj.emitDirType;
        data.numPerUpdate = obj.numPerUpdate;
        data.rate = obj.rate;

        data.loop = obj.loop;
        data.duration = obj.duration;
        data.delay = obj.delay;

        data.pos = float3.zero;
        data.dir = new float3(0, 1, 0);

        return data;
    }
}

