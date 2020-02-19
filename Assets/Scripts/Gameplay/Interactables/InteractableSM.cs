using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class InteractableSM
{
    private InteractableBase carryingInteractable = null;

    public bool useInteractable(InteractableBase interactable)
    {
        if (interactable.CanCarry)
        {
            if (interactable == carryingInteractable)
            {
                carryingInteractable = null;
                return true;
            }
        }
        else
        {
            if (carryingInteractable == null)
                return true;
        }
        return false;
    }

    public bool carryInteractable(InteractableBase interactable)
    {
        if (carryingInteractable == null)
        {
            carryingInteractable = interactable;
            return true;
        }

        return false;
    }

    public bool dropInteractable(InteractableBase interactable)
    {
        if (carryingInteractable != null)
        {
            carryingInteractable = null;
            return true;
        }

        return false;
    }
}
