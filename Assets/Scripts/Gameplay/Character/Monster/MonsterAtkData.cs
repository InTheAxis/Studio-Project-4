using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]
public class MonsterAtkData : MonoBehaviour
{
    [SerializeField]
    private int dmg = 1;
    [SerializeField]
    private LayerMask mask;

    private Collider c;

    private void Awake()
    {
        c = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CharHitBox hitbox = other.transform.root.gameObject.GetComponent<CharHitBox>();
        if (hitbox)
        {
            float dot = Vector3.Dot(hitbox.transform.forward, (transform.position - hitbox.transform.position));
            PhotonView view = PhotonView.Get(hitbox);
            if (view)
                view.RPC("takeDmgRPC", RpcTarget.Others, dmg, dot);
            hitbox.OnHit?.Invoke(dmg, dot);
            c.enabled = false;
            Debug.LogFormat("{0} hit player", gameObject.name);
        }
          else
            Debug.LogFormat("herllo");
    }
}
