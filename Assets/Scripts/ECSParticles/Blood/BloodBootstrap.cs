using UnityEngine;
using Unity.Entities;
public struct BloodTag : IComponentData { }
public class BloodBootstrap : ParticleBootstrap
{
    [Header("Emit Source and Direction")]
    //[SerializeField]
    private Transform emitter;
    private CharHitBox charHitbox;

    private void Start()
    {
        Init(typeof(BloodTag));

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
        ECSParticles.Blood.EmitterCleanUpJobSystem cleanup = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ECSParticles.Blood.EmitterCleanUpJobSystem>();
        cleanup.Update();
    }

    private void LateUpdate()
    {
        if (emitter == null)
        {
            emitter = GameManager.playerObj?.transform;
            if (emitter)
                charHitbox = emitter.GetComponent<CharHitBox>();
            else
                return;
        }

        SetEmitterSource(emitter.position, emitter.forward);
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.B))
            BloodSpray(0, 0);
#endif
    }

    private void BloodSpray(int dmg, float dot) 
    {
        Debug.Log("Blood should Spray");
        Emit();
    }
}