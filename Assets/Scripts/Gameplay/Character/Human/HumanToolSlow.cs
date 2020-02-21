using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanToolSlow : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CharTPController charControl;

    [Header("Tool Settings")]
    [SerializeField]
    private float cooldownTime;
    [SerializeField]
    private float slowFactor;

    [Header("Overlap Detection Settings")]
    [SerializeField]
    private float range;
    [SerializeField]
    private float radius;
    [SerializeField]
    private LayerMask include;


    private bool pressed;
    private float cooldown;


    private void Update()
    {
        pressed = Input.GetAxisRaw("Fire2") != 0;
    }

    private void FixedUpdate()
    {
        if (pressed && cooldown <= 0)
        {
            cooldown = cooldownTime;
            Vector3 lookDir = charControl.lookDir.normalized;
            Vector3 origin = charControl.position + lookDir * radius;
            Vector3 dest = origin + lookDir * range;
            Rigidbody[] rbs = RigidBodyExt.GetRigiBodiesInCapsule(origin, dest, radius, include);
            RigidBodyExt.SlowVel(rbs, slowFactor);
        }
        cooldown -= Time.deltaTime;
    }

#if UNITY_EDITOR

    public bool drawDebug = false;
    private void OnDrawGizmos()
    {
        if (!drawDebug)
            return;
        Vector3 lookDir = charControl.lookDir.normalized;
        Gizmos.DrawWireSphere(charControl.position + lookDir * radius, radius);
        Gizmos.DrawWireSphere(charControl.position + lookDir * (radius + range), radius);
    }
#endif
}
