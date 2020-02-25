using UnityEngine;

public class CharHitBox : MonoBehaviour
{
    public System.Action<int> OnHit;
    public bool hit { private set; get; }

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
