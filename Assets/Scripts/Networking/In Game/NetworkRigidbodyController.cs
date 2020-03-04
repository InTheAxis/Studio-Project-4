using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class NetworkRigidbodyController : MonoBehaviourPun
{
    public static NetworkRigidbodyController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("NetworkRigidbodyController already instantiated! This should not happen");
    }

    public void setVel(Rigidbody rb, Vector3 vel)
    {
        PhotonView view = PhotonView.Get(rb);
        if (view != null)
            setVel(view.ViewID, vel);
        else
        {
            Debug.LogWarning("NetworkRigidbodyController: No PhotonView found for obj " + rb.gameObject.name + "! Setting value locally...");
            rb.velocity = vel;
        }
    }
    [PunRPC]
    public void setVel(int photonViewID, Vector3 vel)
    {
        PhotonView view = PhotonView.Find(photonViewID);
        if (view.IsMine || (view.Owner == null && PhotonNetwork.IsMasterClient))
            view.GetComponent<Rigidbody>().velocity = vel;
        else
        {
            if (view.Owner != null)
                photonView.RPC("setVel", photonView.Owner, view.ViewID, vel);
            else
                photonView.RPC("setVel", RpcTarget.MasterClient, view.ViewID, vel);
        }
    }

    public void applyForce(Rigidbody rb, Vector3 f, ForceMode mode = ForceMode.Impulse)
    {
        PhotonView view = PhotonView.Get(rb);
        if (view != null)
            applyForce(view.ViewID, f, (byte)mode);
        else
        {
            Debug.LogWarning("NetworkRigidbodyController: No PhotonView found for obj " + rb.gameObject.name + "! Setting value locally...");
            // Old: rb.AddForce(f, mode);
            //rb.velocity += f;
            rb.AddForce(f * rb.mass * 0.40f, mode);
        }
    }
    [PunRPC]
    public void applyForce(int photonViewID, Vector3 f, byte mode)
    {
        PhotonView view = PhotonView.Find(photonViewID);
        if (view.IsMine || (view.Owner == null && PhotonNetwork.IsMasterClient))
        {
            Rigidbody rb = view.GetComponent<Rigidbody>();
            rb.AddForce(f * rb.mass * 0.40f, (ForceMode)mode);
        }
            //view.GetComponent<Rigidbody>().velocity += f;
        //Old: view.GetComponent<Rigidbody>().AddForce(f * rb., (ForceMode)mode);
        else
        {
            if (view.Owner != null)
                photonView.RPC("applyForce", photonView.Owner, view.ViewID, f, mode);
            else
                photonView.RPC("applyForce", RpcTarget.MasterClient, view.ViewID, f, mode);
        }
    }
}
