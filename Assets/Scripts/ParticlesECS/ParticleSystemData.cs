using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[CreateAssetMenu]
public class ParticleSystemDataObj : ScriptableObject
{
    public bool enabledOnAwake;
    public int numPerUpdate;
    public float rate;
};

public struct ParticleEmitTag : IComponentData { } //to label whether to emit
public struct ParticleSystemData : IComponentData 
{
    public int numPerUpdate;
    public float rate;

    public static ParticleSystemData Create(ParticleSystemDataObj obj)
    {
        ParticleSystemData data = new ParticleSystemData();

        data.numPerUpdate = obj.numPerUpdate;
        data.rate = obj.rate;

        return data;
    }
}

