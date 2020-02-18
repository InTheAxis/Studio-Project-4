using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    public PlayerHealth health;
    public LayerMask ignore;
    public float hitTime = 0.1f;
    public bool hit { get { return triggered; } }

    [HideInInspector]
    public bool triggered; //in case you want override

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && CheckIfIgnored(other.gameObject.layer))
        {
            DamageData ddata = other.GetComponent<DamageData>();
            if (ddata)
                health.TakeDmg(ddata.dmg);
            triggered = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        triggered = false;   
    }

    private bool CheckIfIgnored(int layerToCheck)
    { 
        //ignore bits AND (1 bit shifted to where layer bit is)
        //if 0 means none matched, which means ignored
        return (ignore & (1 << layerToCheck)) == 0;
    }
}
