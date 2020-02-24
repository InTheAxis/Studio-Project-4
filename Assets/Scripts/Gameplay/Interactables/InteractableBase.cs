using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class InteractableBase : MonoBehaviour
{
    [Tooltip("Whether to call interact() per button press or constantly while button is down")]
    [SerializeField]
    private bool allowConstantInteraction = false;
    public bool AllowConstantInteraction { get => allowConstantInteraction; }
    [Tooltip("Allow player to interact with this interactable even after they've looked away for a brief time")]
    [SerializeField]
    private bool lenientInteraction = true;
    public bool LenientInteraction { get => lenientInteraction; }
    [SerializeField]
    private bool canCarry = false;
    public bool CanCarry { get => canCarry; }

    public abstract string getInteractableName();

    [SerializeField]
    private Vector3 positionOffsetWhileCarry;
    public Vector3 PositionOffsetWhileCarry { get => positionOffsetWhileCarry; }

    public abstract void interact();
    public virtual string getUncarriedTooltip()
    {
        return "Press [E] to " + (canCarry ? "carry " + getInteractableName() : "");
    }
    public virtual string getCarriedTooltip()
    {
        return "Press [E] to drop " + getInteractableName() + "\nPress [F] to ";
    }

    protected void destroyThis()
    {
        //PhotonNetwork.Destroy(PhotonView.Get(this));
        NetworkOwnership.instance.destroy(PhotonView.Get(this));
    }
}
