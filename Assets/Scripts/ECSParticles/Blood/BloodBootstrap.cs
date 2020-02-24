using UnityEngine;
using Unity.Entities;
public struct BloodTag : IComponentData { }
public class BloodBootstrap : ParticleBootstrap
{
    [Header("Emit Source and Direction")]
    [SerializeField]
    private Transform emitter;

    public static BloodBootstrap Instance;
    private void Start()
    {
        Instance = this;

        Init(typeof(BloodTag));
    }
    private void Update()
    {
        if (emitter)
            SetEmitterSource(emitter.position, emitter.forward);

        if (Input.GetKeyDown(KeyCode.P))
            Emit();
    }
}