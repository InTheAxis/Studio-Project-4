using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationSM : MonoBehaviour
{
    public Animator animator;
    public CharTPController charControl;
    public CharHealth health;
    public CharHitBox hitbox;
    public MonsterChargeAtk chargeAtk;


    private DestructibleController destructible;
    private CharLookTargetController lookTargetCtrl;

    private void Awake()
    {
        destructible = FindObjectOfType<DestructibleController>();
        lookTargetCtrl = FindObjectOfType<CharLookTargetController>();
    }

    private void OnEnable()
    {
        if (!charControl.photonView.IsMine && Photon.Pun.PhotonNetwork.IsConnected)
            return;
        hitbox.OnHit += OnHit;
        destructible.pullStatus += AttackHold;
        destructible.throwStatus += AttackRelease;
        //lookTargetCtrl.showMap += MiniMapToggle;
    }

    private void OnDisable()
    {
        if (!charControl.photonView.IsMine && Photon.Pun.PhotonNetwork.IsConnected)
            return;
        hitbox.OnHit -= OnHit;
        destructible.pullStatus -= AttackHold;
        destructible.throwStatus -= AttackRelease;
        //lookTargetCtrl.showMap -= MiniMapToggle;
    }

    private void LateUpdate()
    {
        if (!charControl.photonView.IsMine && Photon.Pun.PhotonNetwork.IsConnected)
            return;

        //if (Input.GetMouseButtonDown(0))
        //    MiniMapToggle(true);
        //if (Input.GetMouseButtonDown(1))
        //    MiniMapToggle(false);
        //if (Input.GetMouseButtonDown(0))
        //    IsSabotaging();
        //if (Input.GetMouseButtonUp(0))
        //    SabotagingDone(true);

        //if (Input.GetMouseButtonDown(0))
        //    AttackHold(true);
        //if (Input.GetMouseButtonUp(0))
        //    AttackRelease();

        //if (Input.GetMouseButtonDown(0))
        //    OnHit(0, -1);

        CalculateState();
    }

    private void AttackHold(bool b)
    {
        if (!b)
            return;
        Trigger("attack");
        Boolean("attackHolding", true);
    }

    private void AttackRelease()
    {
        Boolean("attackHolding", false);
        animator.ResetTrigger("attack");
    }
    private void OnHit(int i, float dot)
    {
        if (!health.dead && dot < 0)
            Trigger("hit");
    }

    public void IsSabotaging()
    {
        Trigger("sabo");
        animator.SetInteger("saboDone", -1);
    }

    public void SabotagingDone(bool finished)
    { 
        animator.SetInteger("saboDone", finished ? 1 : 0);
        animator.ResetTrigger("sabo");
    }

    //private void MiniMapToggle(bool b)
    //{
    //    //if (!b)
    //    //    return;
    //    animator.SetLayerWeight(1, 1);
    //    Trigger("map");
    //}

    private void CalculateState()
    {
        Boolean("charge", chargeAtk.isCharging);
        Boolean("dead", health.dead);
        Boolean("moving", charControl.displacement > 0);
        //Boolean("crouching", charControl.crouchChk.crouching);
        //Boolean("falling", charControl.jumpChk.airborne && charControl.velY < -1f);
        //Boolean("jumping", charControl.jumpChk.airborne && charControl.velY > 0);
    }

    private void Boolean(string next, bool b)
    {
        animator.SetBool(next, b);
    }

    private void Trigger(string next)
    {
        animator.ResetTrigger(next);
        animator.SetTrigger(next);
    }
}
