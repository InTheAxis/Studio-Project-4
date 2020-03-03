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
    //public bool damaging;
    public bool damaging { private set; get; }

    [SerializeField]
    [Tooltip("Sets to not damaging when object stop moving \n Set this to true for throwables")]
    private bool autoSetNotDamaging = true;
    
    private Rigidbody rb;
    private bool moving;
    private GameObject ignoreGo;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(damaging);
            stream.SendNext(ignoreGo.name);
        }
        else
        {
            damaging = (bool)stream.ReceiveNext();
            ignoreGo = GameObject.Find((string)stream.ReceiveNext());
        }
    }

    private void Start()
    {
        damaging = false;
        moving = false;
        rb = GetComponent<Rigidbody>();
    }

    public void SetIsDamaging(GameObject ignore = null)
    {
        Debug.LogFormat("{0} is now damaging, ignoring {1}", gameObject.name, ignore.name);
        damaging = true;
        ignoreGo = ignore;
    }

    public void SetNotDamaging()
    {
        Debug.LogFormat("{0} is not damaging", gameObject.name);
        damaging = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (damaging && collision.gameObject != ignoreGo)
        {
            CharHitBox hitbox = collision.gameObject.GetComponent<CharHitBox>();
            if (hitbox)
            {
                hitbox.OnHit?.Invoke(dmg, Vector3.Dot(hitbox.transform.forward, (transform.position - hitbox.transform.position)));
                damaging = false;
                Debug.LogFormat("{0} hit player on layer", gameObject.name);
            }
        }
        if (damaging && collision.gameObject == ignoreGo)
        {
            Debug.LogFormat("Ignored, because ignoreGo is {0}", ignoreGo?.name);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (!autoSetNotDamaging)
            return;

        moving = rb && rb.velocity.sqrMagnitude > 4;

        if (damaging && !moving)
        {
            damaging = false;
            Debug.LogFormat("{0} is no longer damaging", gameObject.name);
        }
    }
}
