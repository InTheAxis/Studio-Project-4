using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HumanToolPull : MonoBehaviour
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
            charControl.photonView.RPC("playVfxRpcPull", RpcTarget.All, true);
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
                clone.transform.localPosition = new Vector3(0.0f, 0.0f, 15.0f);
                clone.transform.forward = -transform.forward;
                Destroy(clone, clone.GetComponent<ParticleSystem>().main.duration);
                charControl.photonView.RPC("playVfxRpcPull", RpcTarget.All, false);
                //playVFX = false;
            }

            cooldown = cooldownTime;
            Rigidbody[] rbs = RigidBodyExt.GetRigiBodiesInSphere(charControl.position, radius, include);
            RigidBodyExt.PullTowards(rbs, charControl.position, forceMagnitude);
        }
        cooldown -= Time.deltaTime;
    }

    [PunRPC]
    private void playVfxRpcPull(bool b)
    {
        playVFX = b;
        GetComponent<AudioController>().Play("push0");
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