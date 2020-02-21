using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChargeAtk : MonoBehaviour
{
    [SerializeField]
    private CharTPController charControl;
    [SerializeField]
    private MonsterEnergy ener;
    [SerializeField]
    private MonsterChargeCheck chargeChk;
    [SerializeField]
    private float enerConsumption = 10;
    [SerializeField]
    private float duration = 1;
    [SerializeField]
    private float chargeDist = 20;
    [SerializeField]
    private float delay = 0.5f;
    [SerializeField]
    private float acceleration = 0.05f;

    private IEnumerator chargeCorr;

    private void Start()
    {
        chargeCorr = null;
    }

    private void Update()
    {
        if (!charControl.disableKeyInput && Input.GetAxisRaw("ChargeAttack") != 0)
        {
            if (chargeCorr == null && !charControl.jumpChk.airborne)
            {
                chargeCorr = Charge();
                StartCoroutine(chargeCorr);
            }
        }
    }

    private IEnumerator Charge()
    {        
        if (!chargeChk.collided && ener.UseUp(enerConsumption))
        {
            yield return new WaitForSeconds(delay);
            charControl.disableKeyInput = true;
            float timer = 0;
            Vector3 dir = charControl.forward;
            float sqrDist = chargeDist * chargeDist;
            float speed = chargeDist / duration;
            Vector3 pos = charControl.position;
            while (timer < duration && !chargeChk.collided)
            {
                timer += Time.deltaTime;
                speed += acceleration * Time.deltaTime;
                charControl.AddForce(dir * speed, ForceMode.Acceleration);

                if (Mathf.Abs((pos - charControl.position).sqrMagnitude - sqrDist) <= 0.1f)
                    break;

                yield return null;
            }
            charControl.disableKeyInput = false;
        }        
        yield return null;
        chargeCorr = null;
    }
}
