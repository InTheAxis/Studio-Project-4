using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class InteractableRevive : InteractableBase
{
    [SerializeField]
    private float timeToFinishInteraction = 2.0f;

    private bool wasInteracting = false;
    private float interactTime = 0.0f;

    private bool interactedOnce = false;
    // In the process of being destroyed through Destructible script. Don't interact anymore once set
    private bool isDestroyed = false;


    [Header("VFX")]
    [SerializeField]
    private ParticleSystem vfx = null;

    private PhotonView thisView;

    private HumanAnimationSM humanAnim = null;
    private MonsterAnimationSM monsterAnim = null;
    
    private int playerViewId = -1;

    private void Start()
    {
        thisView = PhotonView.Get(this);
    }
    public override string getInteractableName() { return "Revive"; }

    public override void interact()
    {
        if (!Photon.Pun.PhotonNetwork.IsMasterClient)
            return;

        if (isDestroyed)
            return;

        wasInteracting = true;
        Debug.Log("Reviving");

        if (!vfx.isEmitting)
            vfx.Play();
    }

    private void Update()
    {
        if (humanAnim == null || monsterAnim == null)
        {
            if (GameManager.playerObj)
            {
                humanAnim = GameManager.playerObj.GetComponent<HumanAnimationSM>();
                monsterAnim = GameManager.playerObj.GetComponent<MonsterAnimationSM>();
            }
        }
    }

    private void LateUpdate()
    {
        if (GameManager.playerObj)
            GameManager.playerObj.GetComponent<CharTPController>().disableKeyInput = wasInteracting;
        if (isDestroyed)
            return;

        // Is interacting this frame
        if (wasInteracting)
        {
            Debug.Log("Reset Interact");
            wasInteracting = false;
            interactTime += Time.deltaTime;

            interactedOnce = true;

            if (GameManager.playerObj)
            { 
                GameManager.playerObj.GetComponent<CharTPController>().disableKeyInput = true;
            }

            if (humanAnim)
                humanAnim.IsSabotaging();

            if (interactTime >= timeToFinishInteraction) // Done interacting
            {
                interactTime = timeToFinishInteraction;
                Debug.Log("Revive interaction finished!");
                if (humanAnim)
                    humanAnim.SabotagingDone(true);
                thisView.RPC("revivePlayer", RpcTarget.Others, playerViewId);
                destroyThis(); // Can only be called inside interact
            }
        }
        else // Reset timer
        {
            if (interactedOnce)
            {
                interactedOnce = false;

                if (GameManager.playerObj)
                { 
                    GameManager.playerObj.GetComponent<CharTPController>().disableKeyInput = false;
                }
                if (humanAnim)
                    humanAnim.SabotagingDone(false);
            }

            if (vfx.isEmitting)
                vfx.Stop();

            interactTime = 0.0f;
            interactDone = false;
        }
    }

    public override string getUncarriedTooltip()
    {
        if (!Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            if (wasInteracting)
                return "Reviving " + Mathf.RoundToInt(interactTime / timeToFinishInteraction * 100.0f) + "%";
            else
                return base.getUncarriedTooltip() + "Revive teammate";
        }
        else return "";
    }

    public void SetPlayerToRevive(int _viewId)
    {
        playerViewId = _viewId;
    }

    [PunRPC]
    private void revivePlayer(int viewID, PhotonMessageInfo messageInfo)
    {
        //respawn player
        CharTPController target = null;
        foreach (var p in CharTPController.PlayerControllerRefs)
        {
            if (viewID == p.controller.photonView.ViewID)
                target = p.controller;
        }
        if (target)
        { 
            target.GetComponent<CharHealth>().Respawn(1);
            Debug.LogFormat("Respawned {0}", target.name);
        }
    }
}
