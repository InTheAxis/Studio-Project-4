using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableGeneric : InteractableBase
{
    public override void interact()
    {
        Debug.Log("Interacted with generic interactable");
        destroyThis();
    }
}
