using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharHitBox : MonoBehaviour
{
    [SerializeField]
    private LayerMask ignore;
    [SerializeField]
    private float hitTime = 0.1f;

    public System.Action<int> OnHit;

    public bool hit { get { return triggered; } }

    [HideInInspector]
    public bool triggered; //in case you want override

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && LayerMaskExt.CheckIfNotIgnored(ignore, other.gameObject.layer))
        {
            DamageData ddata = other.GetComponent<DamageData>();
            if (ddata)
                OnHit(ddata.dmg);
            triggered = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        triggered = false;   
    }
}
