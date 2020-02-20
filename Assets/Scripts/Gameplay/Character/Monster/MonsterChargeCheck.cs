using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChargeCheck : MonoBehaviour
{
    public LayerMask ignore;
    public bool collided { private set; get; }

    private void Start()
    {
        collided = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMaskExt.CheckIfNotIgnored(ignore, other.gameObject.layer))
            collided = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (LayerMaskExt.CheckIfNotIgnored(ignore, other.gameObject.layer))
            collided = false;
    }
}
