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

    // To be used only by master client
    // One SM is created per player
    private Dictionary<int, InteractableSM> interactableSMs = new Dictionary<int, InteractableSM>();

    private SuccFailWrapper<int, System.Action<InteractableBase>> requests = new SuccFailWrapper<int, System.Action<InteractableBase>>();

    private void Update()
    {
        foreach (var sm in interactableSMs)
            sm.Value.checkState();
    }

    public void useInteractable(InteractableBase interactable, System.Action<InteractableBase> useFunc, System.Action<InteractableBase> failFunc)
    {
        if (PhotonNetwork.IsMasterClient)
            queryMasterInteractable(interactable,
                (sm) => { return sm.useInteractable(interactable, PhotonNetwork.LocalPlayer); },
                useFunc, failFunc);
        else
            sendRequest(interactable, "queryUse", useFunc, failFunc);
    }
    public void constantUseInteractable(InteractableBase interactable, System.Action<InteractableBase> constantUseFunc, System.Action<InteractableBase> failFunc)
    {
        if (PhotonNetwork.IsMasterClient)
            queryMasterInteractable(interactable,
                (sm) => { return sm.constantUseInteractable(interactable, PhotonNetwork.LocalPlayer); },
                (obj) => {
                    NetworkOwnership.instance.transferOwnerAsMaster(PhotonView.Get(obj), PhotonNetwork.MasterClient);
                    constantUseFunc(obj);
                }, failFunc);
        else
            sendRequest(interactable, "queryConstantUse", constantUseFunc, failFunc);
    }
    public void releaseConstantUseInteractable(InteractableBase interactable, System.Action<InteractableBase> releaseConstantUseFunc, System.Action<InteractableBase> failFunc)
    {
        if (PhotonNetwork.IsMasterClient)
            queryMasterInteractable(interactable,
                (sm) => { return sm.releaseConstantUseInteractable(interactable, PhotonNetwork.LocalPlayer); },
                (obj) => {
                    NetworkOwnership.instance.releaseOwnership(PhotonView.Get(obj), null, null);
                    releaseConstantUseFunc(obj);
                }, failFunc);
        else
            sendRequest(interactable, "queryReleaseConstantUse", releaseConstantUseFunc, failFunc);
    }
    public void carryInteractable(InteractableBase interactable, System.Action<InteractableBase> carryFunc, System.Action<InteractableBase> failFunc)
    {
        Debug.Log("Carry query");
        if (PhotonNetwork.IsMasterClient)
            queryMasterInteractable(interactable,
                (sm) => { return sm.carryInteractable(interactable, PhotonNetwork.LocalPlayer); },
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
                (sm) => { return sm.dropInteractable(interactable, PhotonNetwork.LocalPlayer); },
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
            (sm) => { return sm.useInteractable(interactable, messageInfo.Sender); },
            () => { photonView.RPC("onRequestSuccess", messageInfo.Sender, view.ViewID); },
            () => { photonView.RPC("onRequestFail", messageInfo.Sender, view.ViewID); });
    }
    [PunRPC]
    private void queryConstantUse(int viewID, PhotonMessageInfo messageInfo)
    {
        PhotonView view = PhotonView.Find(viewID);
        InteractableBase interactable = view.GetComponent<InteractableBase>();
        queryMasterInteractable(interactable,
            (sm) => { return sm.constantUseInteractable(interactable, messageInfo.Sender); },
            () =>
            {
                NetworkOwnership.instance.transferOwnerAsMaster(view, messageInfo.Sender);
                photonView.RPC("onRequestSuccess", messageInfo.Sender, view.ViewID);
            },
            () => { photonView.RPC("onRequestFail", messageInfo.Sender, view.ViewID); });
    }
    [PunRPC]
    private void queryReleaseConstantUse(int viewID, PhotonMessageInfo messageInfo)
    {
        PhotonView view = PhotonView.Find(viewID);
        InteractableBase interactable = view.GetComponent<InteractableBase>();
        queryMasterInteractable(interactable,
            (sm) => { return sm.releaseConstantUseInteractable(interactable, messageInfo.Sender); },
            () =>
            {
                NetworkOwnership.instance.releaseOwnership(view, null, null);
                photonView.RPC("onRequestSuccess", messageInfo.Sender, view.ViewID);
            },
            () => { photonView.RPC("onRequestFail", messageInfo.Sender, view.ViewID); });
    }
    [PunRPC]
    private void queryCarry(int viewID, PhotonMessageInfo messageInfo)
    {
        PhotonView view = PhotonView.Find(viewID);
        InteractableBase interactable = view.GetComponent<InteractableBase>();
        queryMasterInteractable(interactable,
            (sm) => { return sm.carryInteractable(interactable, messageInfo.Sender); },
            () => 
            {
                NetworkOwnership.instance.transferOwnerAsMaster(view, messageInfo.Sender);
                photonView.RPC("onRequestSuccess", messageInfo.Sender, view.ViewID);
            },
            () => { photonView.RPC("onRequestFail", messageInfo.Sender, view.ViewID); });
    }
    [PunRPC]
    private void queryDrop(int viewID, PhotonMessageInfo messageInfo)
    {
        PhotonView view = PhotonView.Find(viewID);
        InteractableBase interactable = view.GetComponent<InteractableBase>();
        queryMasterInteractable(interactable,
            (sm) => { return sm.dropInteractable(interactable, messageInfo.Sender); },
            () => {
                NetworkOwnership.instance.releaseOwnership(view, null, null);
                photonView.RPC("onRequestSuccess", messageInfo.Sender, view.ViewID);
            },
            () => { photonView.RPC("onRequestFail", messageInfo.Sender, view.ViewID); });
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
    [PunRPC]
    private void onRequestFail(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        System.Action<InteractableBase> func;
        requests.onFailure(view.ViewID, out func);
        func?.Invoke(view.GetComponent<InteractableBase>());
    }
}
