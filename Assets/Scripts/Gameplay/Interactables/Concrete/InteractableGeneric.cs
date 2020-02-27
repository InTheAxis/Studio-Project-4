using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableGeneric : InteractableBase
{
    public override string getInteractableName() { return "Generic"; }

    public override void interact()
    {
        Debug.Log("Interacted with generic interactable");
        destroyThis();
    }

    public override string getCarriedTooltip()
    {
        return base.getCarriedTooltip() + "Use Generic";
    }
}
