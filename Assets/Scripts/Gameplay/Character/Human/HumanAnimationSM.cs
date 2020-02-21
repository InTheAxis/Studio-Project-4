using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimationSM : MonoBehaviour
{
    public Animator animator;
    public CharTPController charControl;
    public CharHealth health;
    public CharHitBox hitbox;

    private string curState;
    
    private void Start()
    {
        curState = "idle";
    }


    private void LateUpdate()
    {
        CalculateState();
    }


    private void CalculateState()
    {
        //TODO Improve this if it gets too messy

        if (curState == "died" && !health.dead)
            StateChange("idle");

        if (health.dead)
            StateChange("died");
        //else if (hitbox.hit)
        //    StateChange("hit");
        else if (charControl.jumpChk.airborne)
        {
            if (charControl.velY > 0)
            {
                //if (charControl.velY < charControl.jumpForce)
                //    StateChange("hang");
                //else
                    StateChange("jump");
            }
            else
                //StateChange("fall");
              StateChange("jump");
        }
        else if (charControl.crouchChk.crouching)
        {
            if (charControl.displacement > 0)
                StateChange("crouchWalk");
            else
                StateChange("crouch");
        }
        else if (charControl.displacement > 0)
        {
            StateChange("walk");
        }
        else
            StateChange("idle");
    }

    public void StateChange(string next)
    {
        if (curState == next)
            return;
        animator.ResetTrigger(curState);
        curState = next;
        animator.SetTrigger(curState);
        Debug.Log("Human anim state: " + curState);
    }
}
