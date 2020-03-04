using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkOwnership : MonoBehaviourPunCallbacks
{
    public static NetworkOwnership instance = null;

    private SuccFailCallbackWrapper<PhotonView, PhotonView> ownershipRequests = new SuccFailCallbackWrapper<PhotonView, PhotonView>();

    public enum PROCESS_RESULT
    {
        NOT_SUPPORTED,
        ACCEPT,
        REJECT
    }
    public delegate PROCESS_RESULT OwnershipListener(PhotonView view, Player requestingPlayer);
    public OwnershipListener ownershipListener;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("NetworkOwnership instantiated more than once! This should not happen");

    }

    public static bool objectIsOwned(PhotonView view)
    {
        Debug.Log("Master ID " + PhotonNetwork.MasterClient.ActorNumber + " Local ID " + PhotonNetwork.LocalPlayer.ActorNumber + " View Owner ID " + view.Owner?.ActorNumber);
        return (view.Owner == null && PhotonNetwork.IsMasterClient) || (view.Owner?.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    public void destroy(PhotonView obj)
    {
        if ((obj.Owner == null && PhotonNetwork.IsMasterClient) || (obj.Owner != null && obj.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber))
            PhotonNetwork.Destroy(obj);
        else
            photonView.RPC("rpcDestroy", obj.Owner, obj.ViewID);
    }
    [PunRPC]
    private void rpcDestroy(int viewID)
    {
        PhotonNetwork.Destroy(PhotonView.Find(viewID));
    }

    // Never tested. No guarantees that this works lol
    public void requestOwner(PhotonView obj, System.Action<PhotonView> successCallback, System.Action<PhotonView> failCallback)
    {
        Debug.Log("Request ownership of PhotonView " + obj.ViewID + " dispatched by Player " + PhotonNetwork.LocalPlayer.ActorNumber);

        // Don't request if obj is already owned
        if (obj.Owner.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Debug.Log("Object is already owned by this player.");
            successCallback?.Invoke(obj);
            return;
        }

        // If this client is master, always allow takeover
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Request is by master. Apply transfer immediately.");
            obj.TransferOwnership(PhotonNetwork.LocalPlayer);
            successCallback?.Invoke(obj);
            return;
        }

        // Send the RPC if request hasn't been made
        if (ownershipRequests.Add(obj, successCallback, failCallback))
        {
            Debug.Log("Request sent to Player " + (obj.Owner?.ActorNumber ?? PhotonNetwork.MasterClient.ActorNumber));
            photonView.RPC("requestReceived", obj.Owner ?? PhotonNetwork.MasterClient, obj.ViewID);
        }
        else // Don't request again if already requested
        {
            Debug.Log("Request already in transit. Rejecting.");
            failCallback?.Invoke(obj);
        }
    }

    public void transferOwnerAsMaster(PhotonView obj, Player toTransferTo)
    {
        Debug.Log("Transferring PhotonView " + obj.ViewID + " to Player " + toTransferTo.ActorNumber);

        if (PhotonNetwork.IsMasterClient)
            obj.TransferOwnership(toTransferTo);
        else
            Debug.Log("This Player " + PhotonNetwork.LocalPlayer.ActorNumber + " is not the master!");
    }

    public void releaseOwnership(PhotonView obj, System.Action<PhotonView> successCallback, System.Action<PhotonView> failCallback)
    {
        //Debug.Log("Release of ownership of PhotonView " + obj.ViewID + " dispatched by Player " + PhotonNetwork.LocalPlayer.ActorNumber);
        
        // Make sure this object is actually owned by this client
        if (obj.Owner == null || obj.Owner.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Debug.LogWarning("This PhotonView is not owned by " + obj.Owner?.ActorNumber + ", not by this client!");
            failCallback?.Invoke(obj);
            return;
        }

        if (obj.IsSceneView) // Scene Object
        {
            //Debug.Log("Released to scene");
            obj.TransferOwnership(0);
            successCallback?.Invoke(obj);
            return;
        }
        else // Assume master client object
        {
            Debug.Log("Transferred ownership to master client " + PhotonNetwork.MasterClient.ActorNumber);
            obj.TransferOwnership(PhotonNetwork.MasterClient);
            successCallback?.Invoke(obj);
            return;
        }
    }

    [PunRPC]
    private void requestReceived(int viewID, PhotonMessageInfo messageInfo)
    {
        Debug.Log("Request ownership for PhotonView " + viewID + " received by Player " + PhotonNetwork.LocalPlayer.ActorNumber);

        PhotonView obj = PhotonView.Find(viewID);
        foreach (var f in ownershipListener.GetInvocationList())
            switch ((PROCESS_RESULT)f.DynamicInvoke(obj, messageInfo.Sender))
            {
                case PROCESS_RESULT.ACCEPT:
                    Debug.Log("Request accepted. Dispatching accept message to Player " + messageInfo.Sender.ActorNumber);
                    obj.TransferOwnership(messageInfo.Sender.ActorNumber);
                    photonView.RPC("requestAccepted", messageInfo.Sender, viewID);
                    return;
                case PROCESS_RESULT.REJECT:
                    Debug.Log("Request rejected. Dispatching reject message to Player " + messageInfo.Sender.ActorNumber);
                    photonView.RPC("requestRejected", messageInfo.Sender, viewID);
                    return;
            }

        Debug.Log("Request UNHANDLED. Dispatching reject message to Player " + messageInfo.Sender.ActorNumber);
        photonView.RPC("requestRejected", messageInfo.Sender, viewID);
    }

    [PunRPC]
    private void requestAccepted(int viewID)
    {
        Debug.Log("Request accept message received for PhotonView " + viewID + " by Player " + PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonView obj = PhotonView.Find(viewID);
        ownershipRequests.invokeSuccess(obj, obj);
    }

    [PunRPC]
    private void requestRejected(int viewID)
    {
        Debug.Log("Request reject message received for PhotonView " + viewID + " by Player " + PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonView obj = PhotonView.Find(viewID);
        ownershipRequests.invokeFailure(obj, obj);
    }
}
