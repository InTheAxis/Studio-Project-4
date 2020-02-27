using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChargeCheck : MonoBehaviour
{
    public LayerMask ignore;

    public System.Action Collided;
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMaskExt.CheckIfNotIgnored(ignore, other.gameObject.layer))
            Collided();
    }
}
