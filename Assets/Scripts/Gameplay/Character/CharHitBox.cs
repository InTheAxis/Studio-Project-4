using UnityEngine;

public class CharHitBox : MonoBehaviour
{
    public System.Action<int> OnHit;

    public bool hit { get { return triggered; } }

    [HideInInspector]
    public bool triggered; //in case you want override

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            DamageData ddata = other.GetComponent<DamageData>();
            if (ddata)
                OnHit?.Invoke(ddata.dmg);
            triggered = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        triggered = false;   
    }
}
