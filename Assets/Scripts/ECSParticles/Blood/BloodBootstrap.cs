using UnityEngine;
using Unity.Entities;
public struct BloodTag : IComponentData { }
public class BloodBootstrap : ParticleBootstrap
{
    [Header("Emit Source and Direction")]
    [SerializeField]
    private Transform emitter;

    private ECSParticles.Blood.EmitterCleanUpJobSystem cleanup;

    private void Start()
    {
        Init(typeof(BloodTag));

        cleanup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ECSParticles.Blood.EmitterCleanUpJobSystem>();
    }

    protected override void DestroyEntities()
    {
        cleanup.Update();
    }

    private void LateUpdate()
    {
        if (emitter)
            SetEmitterSource(emitter.position, emitter.forward);
    }
}