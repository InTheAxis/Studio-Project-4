using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTower : InteractableBase
{
    [SerializeField]
    private float timeToFinishInteractionMonster = 2.0f;
    [SerializeField]
    private float timeToFinishInteractionHuman = 2.0f;
    private float timeToFinishInteraction = 2.0f;

    private bool wasInteracting = false;
    private float interactTime = 0.0f;

    private void Start()
    {
        if (Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            timeToFinishInteraction = timeToFinishInteractionMonster;
        }
        else
        {
            timeToFinishInteraction = timeToFinishInteractionHuman;
        }
    }
    public override string getInteractableName() { return "Tower"; }

    public override void interact()
    {
        wasInteracting = true;
        Debug.Log("Interact");
    }

    private void LateUpdate()
    {
        if (GameManager.playerObj)
            GameManager.playerObj.GetComponent<CharTPController>().disableKeyInput = wasInteracting;

        // Is interacting this frame
        if (wasInteracting)
        {
            Debug.Log("Reset Interact");
            wasInteracting = false;
            interactTime += Time.deltaTime;

            // Done interacting
            if (interactTime >= timeToFinishInteraction)
            {
                interactTime = timeToFinishInteraction;
                Debug.Log("Tower destroyed!");
                HumanUnlockTool unlock = GameManager.playerObj.GetComponent<HumanUnlockTool>();
                MonsterEnergy recahrge = GameManager.playerObj.GetComponent<MonsterEnergy>();
                if (unlock)
                {
                    unlock.Unlock(HumanUnlockTool.TYPE.RANDOM);
                    destroyThis(); // Can only be called inside interact
                }
                else if (recahrge)
                {
                    recahrge.RechargePercent(interactTime / timeToFinishInteraction);
                    interactDone = true;
                    Debug.Log("Recharged");
                }
            }
        }
        else // Reset timer
        { 
            interactTime = 0.0f;
            interactDone = false;
        }
    }

    public override string getUncarriedTooltip()
    {
        if (Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            if (wasInteracting)
                return "Recharging " + Mathf.RoundToInt(interactTime / timeToFinishInteraction * 100.0f) + "%";
            else
                return base.getUncarriedTooltip() + "recharge energy from Tower";
        }
        else
        {
            if (wasInteracting)
                return "Destroying Tower " + Mathf.RoundToInt(interactTime / timeToFinishInteraction * 100.0f) + "%";
            else
                return base.getUncarriedTooltip() + "destroy Tower";
        }
    }
}
