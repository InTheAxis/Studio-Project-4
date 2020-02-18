using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class InteractableBase : MonoBehaviour
{
    protected bool canCarry = false;
    public bool CanCarry { get => canCarry; }

    public abstract void interact();

    protected void destroyThis()
    {
        PhotonNetwork.Destroy(PhotonView.Get(this));
    }
}
