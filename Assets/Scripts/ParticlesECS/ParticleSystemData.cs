using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[CreateAssetMenu]
public class ParticleSystemDataObj : ScriptableObject
{
    public bool enabledOnAwake;
    public int numPerUpdate;
    public float rate;
    [Tooltip("The amount of entities per job")]
    public int numJobBatch;
};

public struct ParticleEmitTag : IComponentData { } //to label whether to emit
public struct ParticleSystemData : IComponentData 
{
    public int numPerUpdate;
    public float rate;
    public int numJobBatch;

    public static ParticleSystemData Create(ParticleSystemDataObj obj)
    {
        ParticleSystemData data = new ParticleSystemData();

        data.numPerUpdate = obj.numPerUpdate;
        data.rate = obj.rate;
        data.numJobBatch = obj.numJobBatch;

        return data;
    }
}

