using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanToolPull : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CharTPController charControl;

    [Header("Tool Settings")]
    [SerializeField]
    private float cooldownTime;
    [SerializeField]
    private float forceMagnitude;

    [Header("Overlap Detection Settings")]
    [SerializeField]
    private float radius;
    [SerializeField]
    private LayerMask include;


    private bool pressed;
    private float cooldown;
    private void OnEnable()
    {
        //init & reset
        cooldown = 0;
        pressed = false;
    }
    private void Update()
    {
        if (!charControl.photonView.IsMine && Photon.Pun.PhotonNetwork.IsConnected)
            return;

        pressed = Input.GetAxisRaw("Fire2") != 0;
    }

    private void FixedUpdate()
    {
        if (!charControl.photonView.IsMine && Photon.Pun.PhotonNetwork.IsConnected)
            return;
        if (pressed && cooldown <= 0)
        {
            cooldown = cooldownTime;
            Rigidbody[] rbs = RigidBodyExt.GetRigiBodiesInSphere(charControl.position, radius, include);
            RigidBodyExt.PullTowards(rbs, charControl.position, forceMagnitude);
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
