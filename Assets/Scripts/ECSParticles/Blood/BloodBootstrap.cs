using UnityEngine;
using Unity.Entities;
public struct BloodTag : IComponentData { }
public class BloodBootstrap : ParticleBootstrap
{
    [Header("Emit Source and Direction")]
    [SerializeField]
    private Transform emitter = null;

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
        if (emitter == null)
        {
            emitter = GameManager.monsterObj?.transform;
            if (emitter == null)
                return;
        }

        SetEmitterSource(emitter.position, emitter.forward);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (isEmitting) StopEmit();
            else Emit();
        }
#endif
    }
}