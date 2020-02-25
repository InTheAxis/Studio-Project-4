using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//acts like a tag and store damage dealt. 
//put this on things that can hit stuff, on same level as collider.

[RequireComponent(typeof(Collider))]
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
        damaging = false;
    }

    public void SetIsDamaging()
    {
        damaging = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        damaging = false;
    }
}
