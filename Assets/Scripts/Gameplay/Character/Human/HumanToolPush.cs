using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HumanToolPush : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CharTPController charControl;

    [Header("VFX")]
    [SerializeField]
    private GameObject vfx = null;

    [Header("Tool Settings")]
    [SerializeField]
    private float cooldownTime;

    [SerializeField]
    private float forceMagnitude;

    [Header("Overlap Detection Settings")]
    [SerializeField]
    private float range;

    [SerializeField]
    private float radius;

    [SerializeField]
    private LayerMask include;

    private bool pressed;
    private float cooldown;
    private bool playVFX = false;

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

        if (Input.GetButtonDown("Fire2"))
        {
            //playVFX = true;
            charControl.photonView.RPC("playVfxRpcPush", RpcTarget.All, true);
        }
    }

    private void FixedUpdate()
    {
        if (!charControl.photonView.IsMine && Photon.Pun.PhotonNetwork.IsConnected)
            return;
        if (pressed && cooldown <= 0)
        {
            /* Play VFX */
            if (playVFX && vfx != null)
            {
                GameObject clone = Instantiate(vfx);
                clone.transform.SetParent(Camera.main.transform);
                clone.transform.forward = -Camera.main.transform.forward;
                clone.transform.localPosition = new Vector3(0.0f, 0.0f, charControl.targetCamDist * 0.5f);
                Destroy(clone, clone.GetComponent<ParticleSystem>().main.duration);
                charControl.photonView.RPC("playVfxRpcPush", RpcTarget.All, false);
                //playVFX = false;
            }

            cooldown = cooldownTime;
            Vector3 lookDir = charControl.lookDir.normalized;
            Vector3 origin = charControl.position + lookDir * radius;
            Vector3 dest = origin + lookDir * range;
            Rigidbody[] rbs = RigidBodyExt.GetRigiBodiesInCapsule(origin, dest, radius, include);
            RigidBodyExt.PushInDirection(rbs, forceMagnitude * lookDir);
        }
        cooldown -= Time.deltaTime;
    }

    [PunRPC]
    private void playVfxRpcPush(bool b)
    {
        playVFX = b;
        GetComponent<PlayerAudioContoller>().Play("push0");
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