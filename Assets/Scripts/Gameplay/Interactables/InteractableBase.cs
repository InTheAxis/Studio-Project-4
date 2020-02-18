using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class InteractableBase : MonoBehaviour
{
    [SerializeField]
    private bool canCarry = false;
    public bool CanCarry { get => canCarry; }

    [SerializeField]
    private Vector3 positionOffsetWhileCarry;
    public Vector3 PositionOffsetWhileCarry { get => positionOffsetWhileCarry; }

    public abstract void interact();

    protected void destroyThis()
    {
        PhotonNetwork.Destroy(PhotonView.Get(this));
    }
}
