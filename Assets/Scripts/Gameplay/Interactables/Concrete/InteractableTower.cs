using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableTower : InteractableBase
{
    [SerializeField]
    private float timeToFinishInteraction = 2.0f;

    private bool wasInteracting = false;
    private float interactTime = 0.0f;

    public override string getInteractableName() { return "Tower"; }

    public override void interact()
    {
        wasInteracting = true;
        Debug.Log("Interact");
    }

    private void LateUpdate()
    {
        // Is interacting this frame
        if (wasInteracting)
        {
            Debug.Log("Reset Interact");
            wasInteracting = false;
            interactTime += Time.deltaTime;

            // Done interacting
            if (interactTime >= timeToFinishInteraction)
            {
                Debug.Log("Tower destroyed!");
                destroyThis(); // Can only be called inside interact
                HumanUnlockTool unlock = GameManager.playerObj.GetComponent<HumanUnlockTool>();
                if (unlock)
                {
                    unlock.Unlock(HumanUnlockTool.TYPE.RANDOM);
                }
            }
        }
        else // Reset timer
            interactTime = 0.0f;
    }

    public override string getUncarriedTooltip()
    {
        if (wasInteracting)
            return "Destroying Tower " + Mathf.RoundToInt(interactTime / timeToFinishInteraction * 100.0f) + "%";
        else
            return base.getUncarriedTooltip() + "destroy Tower";
    }
}
