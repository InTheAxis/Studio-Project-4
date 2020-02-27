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
    [SerializeField]
    private float cameraDist = 12;
    [SerializeField]
    private float cooldown = 1;
    [SerializeField]
    private TrailRenderer trail;

    private float timer;
    private IEnumerator chargeCorr;

    private void Start()
    {
        timer = cooldown + duration;
        chargeCorr = null;
        trail.emitting = false;
    }

    private void OnEnable()
    {
        chargeChk.Collided += HitObstacle;
    }

    private void OnDisable()
    {
        chargeChk.Collided -= HitObstacle;        
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > cooldown + duration && !charControl.disableKeyInput && Input.GetAxisRaw("ChargeAttack") != 0)
        {
            if (chargeCorr == null && !charControl.jumpChk.airborne)
            {
                timer = 0;
                chargeCorr = Charge();
                StartCoroutine(chargeCorr);
            }
        }
    }

    private void HitObstacle()
    {
        if (chargeCorr != null)
            StopCoroutine(chargeCorr);
        chargeCorr = null;
        charControl.disableMovement = false;
        trail.emitting = false;
    }

    private IEnumerator Charge()
    {        
        if (ener.UseUp(enerConsumption))
        {
            yield return new WaitForSeconds(delay);
            charControl.disableMovement = true;
            trail.emitting = true;
            CharTPCamera.Instance.LookAt(0, cameraDist);
            float timer = 0;
            Vector3 dir = charControl.forward + Vector3.down * 0.1f;
            float sqrDist = chargeDist * chargeDist;
            float speed = chargeDist / duration;
            Vector3 pos = charControl.position;
            while (timer < duration && Mathf.Abs((pos - charControl.position).sqrMagnitude - sqrDist) >= 0.1f)
            {
                timer += Time.deltaTime;
                speed += acceleration * Time.deltaTime;
                charControl.rb.AddForce(dir * speed, ForceMode.Acceleration);
                yield return null;
            }
            charControl.disableMovement = false;
            CharTPCamera.Instance.LookAtPlayer();
        }

        chargeCorr = null;
        yield return new WaitForSeconds(0.1f);
        trail.emitting = false;
    }
}
