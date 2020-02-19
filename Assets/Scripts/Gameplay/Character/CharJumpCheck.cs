using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharJumpCheck : MonoBehaviour
{
    public LayerMask ignore;
    public bool airborne { private set; get; }

    public void Jumping()
    {
        airborne = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMaskExt.CheckIfNotIgnored(ignore, other.gameObject.layer))
            airborne = false;        
    }

    private void OnTriggerStay(Collider other)
    {
        if (LayerMaskExt.CheckIfNotIgnored(ignore, other.gameObject.layer))
            airborne = false;
    }
}
