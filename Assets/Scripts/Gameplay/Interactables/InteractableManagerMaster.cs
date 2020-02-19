using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractableManagerMaster : MonoBehaviourPun
{
    public static InteractableManagerMaster instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("InteractableManagerMaster instantiated already! This should not happen");
    }

    // One SM is created per player
    private Dictionary<int, InteractableSM> interactableSMs = new Dictionary<int, InteractableSM>();

    private SuccFailWrapper<int, System.Action<InteractableBase>> requests = new SuccFailWrapper<int, System.Action<InteractableBase>>();

    public void useInteractable(InteractableBase interactable, System.Action<InteractableBase> useFunc, System.Action<InteractableBase> failFunc)
    {
        if (PhotonNetwork.IsMasterClient)
            queryMasterInteractable(interactable,
                (sm) => { return sm.useInteractable(interactable); },
                useFunc, failFunc);
        else
            sendRequest(interactable, "queryUse", useFunc, failFunc);
    }
    public void carryInteractable(InteractableBase interactable, System.Action<InteractableBase> carryFunc, System.Action<InteractableBase> failFunc)
    {
        Debug.Log("Carry query");
        if (PhotonNetwork.IsMasterClient)
            queryMasterInteractable(interactable,
                (sm) => { return sm.carryInteractable(interactable); },
                (obj) => {
                    NetworkOwnership.instance.transferOwnerAsMaster(PhotonView.Get(obj), PhotonNetwork.MasterClient);
                    carryFunc(obj);
                },
                failFunc);
        else
            sendRequest(interactable, "queryCarry", carryFunc, failFunc);
    }
    public void dropInteractable(InteractableBase interactable, System.Action<InteractableBase> dropFunc, System.Action<InteractableBase> failFunc)
    {
        if (PhotonNetwork.IsMasterClient)
            queryMasterInteractable(interactable,
                (sm) => { return sm.dropInteractable(interactable); },
                (obj) => {
                    NetworkOwnership.instance.releaseOwnership(PhotonView.Get(obj), null, null);
                    dropFunc(obj);
                }, failFunc);
        else
            sendRequest(interactable, "queryDrop", dropFunc, failFunc);
    }
    private void sendRequest(InteractableBase interactable, string queryName, System.Action<InteractableBase> succeedFunc, System.Action<InteractableBase> failFunc)
    {
        PhotonView view = PhotonView.Get(interactable);
        if (requests.Add(view.ViewID, succeedFunc, failFunc))
            photonView.RPC(queryName, RpcTarget.MasterClient, view.ViewID);
        else
            Debug.Log("Request already added");
    }

    [PunRPC]
    private void queryUse(int viewID, PhotonMessageInfo messageInfo)
    {
        PhotonView view = PhotonView.Find(viewID);
        InteractableBase interactable = view.GetComponent<InteractableBase>();
        queryMasterInteractable(interactable,
            (sm) => { return sm.useInteractable(interactable); },
            () => { photonView.RPC("onRequestSuccess", messageInfo.Sender, view.ViewID); },
            null);
    }
    [PunRPC]
    private void queryCarry(int viewID, PhotonMessageInfo messageInfo)
    {
        PhotonView view = PhotonView.Find(viewID);
        InteractableBase interactable = view.GetComponent<InteractableBase>();
        queryMasterInteractable(interactable,
            (sm) => { return sm.carryInteractable(interactable); },
            () => 
            {
                NetworkOwnership.instance.transferOwnerAsMaster(view, messageInfo.Sender);
                photonView.RPC("onRequestSuccess", messageInfo.Sender, view.ViewID);
            },
            null);
    }
    [PunRPC]
    private void queryDrop(int viewID, PhotonMessageInfo messageInfo)
    {
        PhotonView view = PhotonView.Find(viewID);
        InteractableBase interactable = view.GetComponent<InteractableBase>();
        queryMasterInteractable(interactable,
            (sm) => { return sm.dropInteractable(interactable); },
            () => { photonView.RPC("onRequestSuccess", messageInfo.Sender, view.ViewID); },
            null);
    }

    private void queryMasterInteractable(InteractableBase interactable, System.Func<InteractableSM, bool> checkFunc, System.Action<InteractableBase> succeedFunc, System.Action<InteractableBase> failFunc)
    {
        if (!interactableSMs.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
            interactableSMs.Add(PhotonNetwork.LocalPlayer.ActorNumber, new InteractableSM());
        if (checkFunc(interactableSMs[PhotonNetwork.LocalPlayer.ActorNumber]))
            succeedFunc(interactable);
        else
            failFunc?.Invoke(interactable);
    }
    private void queryMasterInteractable(InteractableBase interactable, System.Func<InteractableSM, bool> checkFunc, System.Action succeedFunc, System.Action failFunc)
    {
        if (!interactableSMs.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
            interactableSMs.Add(PhotonNetwork.LocalPlayer.ActorNumber, new InteractableSM());
        if (checkFunc(interactableSMs[PhotonNetwork.LocalPlayer.ActorNumber]))
            succeedFunc();
        else
            failFunc?.Invoke();
    }

    [PunRPC]
    private void onRequestSuccess(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        System.Action<InteractableBase> func;
        requests.onSuccess(view.ViewID, out func);
        func?.Invoke(view.GetComponent<InteractableBase>());
    }
}
