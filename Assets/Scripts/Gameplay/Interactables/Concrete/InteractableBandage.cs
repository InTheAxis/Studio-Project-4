using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBandage : InteractableBase
{
    public override string getInteractableName() { return "Bandage"; }

    public override void interact()
    {
        Debug.Log("Interacted with bandage interactable");
        GameManager.playerObj.GetComponent<CharHealth>().Heal(1);
        destroyThis();
    }

    public override string getUncarriedTooltip()
    {
        return base.getUncarriedTooltip() + "Use Bandage";
    }
}
