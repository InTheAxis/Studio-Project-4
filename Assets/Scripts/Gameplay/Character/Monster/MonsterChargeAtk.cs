using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MonsterChargeAtk : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    private CharTPController charControl;

    [SerializeField]
    private MonsterEnergy ener;

    [SerializeField]
    private MonsterChargeCheck chargeChk;

    [SerializeField]
    private float enerConsumption = 10;

    [SerializeField]
    private float duration = 2;

    [SerializeField]
    private float speed = 10;

    [SerializeField]
    private float delay = 0.5f;

    [SerializeField]
    private float acceleration = 0.05f;

    [SerializeField]
    private float cameraDist = 12;

    [SerializeField]
    private float cooldown = 1;

    //[SerializeField]
    //private TrailRenderer trail;
    [SerializeField]
    private Collider attackColl;

    [Header("VFX")]
    [SerializeField]
    private ParticleSystem vfx = null;

    private float timer;
    private IEnumerator chargeCorr;
    private PhotonView thisView;

    public bool isCharging { private set; get; }

    private void Start()
    {
        timer = cooldown + duration;
        chargeCorr = null;
        //trail.emitting = false;

        thisView = PhotonView.Get(this);
        attackColl.enabled = false;
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
        if (!charControl.photonView.IsMine && Photon.Pun.PhotonNetwork.IsConnected)
            return;

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
        charControl.DisableMovement(false);
        charControl.DisableFriction(false);
        isCharging = false;
        //atkData.SetNotDamaging();
        attackColl.enabled = false;
        //trail.emitting = false;
        CharTPCamera.Instance.LookAtPlayer();
    }

    private IEnumerator Charge()
    {
        if (ener.UseUp(enerConsumption))
        {
            yield return new WaitForSeconds(delay);
            thisView.RPC("playVFX", RpcTarget.All, true);
            charControl.DisableMovement(true);
            charControl.DisableFriction(true);
            //trail.emitting = true;
            isCharging = true;
            GetComponent<MonsterAudioController>()?.Charge();
            //atkData.SetIsDamaging(gameObject);
            attackColl.enabled = true;
            CharTPCamera.Instance.LookAt(0, cameraDist);
            float timer = 0;
            Vector3 dir = charControl.forward + Vector3.down * 0.1f;
            float speed = this.speed;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                speed += acceleration * Time.deltaTime;
                charControl.rb.AddForce(dir * speed, ForceMode.Acceleration);
                yield return null;
            }
            charControl.DisableMovement(false);
            charControl.DisableFriction(false);
            CharTPCamera.Instance.LookAtPlayer();
        }

        chargeCorr = null;
        isCharging = false;
        //atkData.SetNotDamaging();
        attackColl.enabled = false;
        yield return new WaitForSeconds(0.1f);
        //trail.emitting = false;
    }

    [PunRPC]
    private void playVFX(bool play)
    {
        if (vfx != null && !vfx.isEmitting)
            vfx.Play();
    }
}