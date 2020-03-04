using UnityEngine;
using Photon.Pun;

public class CharHitBox : MonoBehaviour
{
    public System.Action<int, float> OnHit; //damage, dot product of angle
    //public bool hit { private set; get; }
    private PhotonView photonView;

    private void Start()
    {
        photonView = transform.root.GetComponent<CharTPController>()?.photonView;
        OnHit += onHitVFX;
    }

    private void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha0))
        {
            OnHit?.Invoke(1, -1);
        }
    }

    private void onHitVFX(int damage, float dotProduct)
    {

    }
    //private void Start()
    //{
    //    Collider[] otherCollsOnThisPlayer = transform.root.GetComponentsInChildren<Collider>();
    //    foreach (Collider c in otherCollsOnThisPlayer)
    //        Physics.IgnoreCollision(c, GetComponent<Collider>(), true);
    //}
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (!hit)
    //    {
    //        DamageData ddata = collision.gameObject.GetComponent<DamageData>();
    //        if (ddata && ddata.damaging)
    //            OnHit?.Invoke(ddata.dmg);
    //        hit = true;
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    hit = false;           
    //}
}
