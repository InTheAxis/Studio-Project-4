using UnityEngine;
using Unity.Entities;
public struct BloodTag : IComponentData { }
public class BloodBootstrap : ParticleBootstrap
{
    [Header("Emit Source and Direction")]
    //[SerializeField]
    private Transform emitter;
    private CharHitBox charHitbox;
    private ECSParticles.Blood.EmitterCleanUpJobSystem cleanup;

    private void Start()
    {
        Init(typeof(BloodTag));

        cleanup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ECSParticles.Blood.EmitterCleanUpJobSystem>();

        emitter = GameManager.playerObj?.transform;
        if (emitter)
            charHitbox = emitter.GetComponent<CharHitBox>();
        Debug.Log(charHitbox);
    }

    private void OnEnable()
    {
        if (charHitbox) charHitbox.OnHit += BloodSpray;
    }
    private void OnDisable()
    {
        if (charHitbox) charHitbox.OnHit -= BloodSpray;        
    }

    protected override void DestroyEntities()
    {
        cleanup.Update();
    }

    private void LateUpdate()
    {
        if (emitter)
            SetEmitterSource(emitter.position, emitter.forward);
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.B))
            Emit();
#endif
    }

    private void BloodSpray(int dmg) 
    {
        Emit();
    }
}