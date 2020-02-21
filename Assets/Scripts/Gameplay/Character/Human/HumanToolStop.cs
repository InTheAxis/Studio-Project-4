using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanToolStop : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CharTPController charControl;

    [Header("Tool Settings")]
    [SerializeField]
    private float cooldownTime;

    [Header("Overlap Detection Settings")]
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
            Rigidbody[] rbs = RigidBodyExt.GetRigiBodiesInSphere(charControl.position, radius, include);
            RigidBodyExt.ResetVel(rbs);
        }
        cooldown -= Time.deltaTime;
    }

#if UNITY_EDITOR

    public bool drawDebug = false;
    private void OnDrawGizmos()
    {
        if (!drawDebug)
            return;
        Gizmos.DrawWireSphere(charControl.position, radius);
    }
#endif
}
