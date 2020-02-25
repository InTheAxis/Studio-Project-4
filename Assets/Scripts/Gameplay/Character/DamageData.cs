using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//acts like a tag and store damage dealt. 
//put this on things that can hit stuff, on same level as collider.

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class DamageData : MonoBehaviour, IPunObservable
{
    public int dmg = 1;
    public bool damaging { private set; get; }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(damaging);
        }
        else
        {
            damaging = (bool)stream.ReceiveNext();
        }
    }

    private void Start()
    {
        damaging = true;
    }

    public void SetIsDamaging()
    {
        Debug.LogFormat("{0} is now damaging", gameObject.name);
        damaging = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (damaging)
        {
            CharHitBox hitbox = collision.gameObject.GetComponent<CharHitBox>();
            if (hitbox)
            {
                hitbox.OnHit?.Invoke(dmg);
                //damaging = false;
                Debug.LogFormat("{0} hit player", gameObject.name);
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (damaging && collision.gameObject.CompareTag("Terrain") && GetComponent<Rigidbody>().velocity.sqrMagnitude < 4)
        {
            //damaging = false;
            Debug.LogFormat("{0} is no longer damaging", gameObject.name);
        }
    }
}
