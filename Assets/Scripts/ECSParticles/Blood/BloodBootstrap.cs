using UnityEngine;
using Unity.Entities;
public struct BloodTag : IComponentData { }
public class BloodBootstrap : ParticleBootstrap
{
    public static BloodBootstrap Instance;
    private void Start()
    {
        Instance = this;

        Init(typeof(BloodTag));
    }

    public void BloodSpray(Vector3 pos, Vector3 forward)
    {
        SetEmitterSource(pos, forward);
        Emit();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            BloodSpray(new Vector3(-1, 1, 0), Vector3.right);
            //if (isEmitting) StopEmit();
            //else            Emit();

        //if (Input.GetKeyDown(KeyCode.U))
        //    SetEmitterSource(new Vector3(0, 3, 0), Vector3.down);
        //if (Input.GetKeyDown(KeyCode.I))
        //    SetEmitterSource(new Vector3(-1, 1, 0), Vector3.right);
    }
}