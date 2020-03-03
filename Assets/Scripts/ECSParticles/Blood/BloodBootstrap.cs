using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
public struct BloodTag : IComponentData { }
public class BloodBootstrap : ParticleBootstrap
{
    public static BloodBootstrap Instance = null;
        
    //[SerializeField]
    //private uint numEmitters = 1;

    //public List<Transform> emitters;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Init(typeof(BloodTag));
    }

    protected override void DestroyEntities()
    {
        ECSParticles.Blood.EmitterCleanUpJobSystem cleanup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ECSParticles.Blood.EmitterCleanUpJobSystem>();
        cleanup.Update();
    }

    private void LateUpdate()
    {
        //if (emitters.Count <= 0)
        //    return;
        //if (emitters[0])
        //    SetEmitterSource(emitters[0].position, emitters[0].forward);
    }
}