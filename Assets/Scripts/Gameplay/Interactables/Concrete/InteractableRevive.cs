using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

[RequireComponent(typeof(PhotonView))]
public class InteractableRevive : InteractableBase
{
    [SerializeField]
    private float timeToFinishInteraction = 2.0f;
    [SerializeField]
    private float timeout = 30;

    private bool wasInteracting = false;
    private float interactTime = 0.0f;

    private bool interactedOnce = false;
    // In the process of being destroyed through Destructible script. Don't interact anymore once set
    private bool isDestroyed = false;


    //[Header("VFX")]
    //[SerializeField]
    //private ParticleSystem vfx = null;

    private PhotonView thisView;

    private HumanAnimationSM humanAnim = null;
    private MonsterAnimationSM monsterAnim = null;
    
    private int playerViewId = -1;
    private float timeoutCounter;
    private bool isLocalDead = false;

    private TextMeshProUGUI tmCountdown = null;

    private void Start()
    {
        thisView = PhotonView.Get(this);
        isLocalDead = (playerViewId == GameManager.playerObj.GetPhotonView().ViewID);
        tmCountdown = StateController.getState("Death").transform.Find("Countdown").GetComponent<TextMeshProUGUI>();

        if(tmCountdown == null)
        {
            Debug.LogError("Countdown UI not set!");
            return;
        }
    }
    public override string getInteractableName() { return "Revive"; }

    public override void interact()
    {
        if (Photon.Pun.PhotonNetwork.IsMasterClient)
            return;

        if (isDestroyed)
            return;

        wasInteracting = true;
        //Debug.Log("Reviving");

        //if (!vfx.isEmitting)
        //    vfx.Play();

        if (!BloodBootstrap.Instance.isEmitting)
            thisView.RPC("playEcsVfx", RpcTarget.All, true);

        thisView.RPC("resetTimeoutTimer", RpcTarget.All, true);
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
        timeoutCounter += Time.deltaTime;
        

        if (timeoutCounter > timeout)
        {
            if(isLocalDead)
            {
                int timer = (int)timeoutCounter;
                timer = Mathf.Max(0, timer);
                tmCountdown.text = timer.ToString() + "s";
            }
            destroyThis(); 
        }
    }

    private void LateUpdate()
    {
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
                CharTPController cc = GameManager.playerObj.GetComponent<CharTPController>();
                cc.DisableKeyInput(true);
                cc.DisableMouseInput(true);
            }

            if (humanAnim)
                humanAnim.IsSabotaging();

            if (interactTime >= timeToFinishInteraction) // Done interacting
            {
                if (BloodBootstrap.Instance.isEmitting) 
                    thisView.RPC("playEcsVfx", RpcTarget.All, false);

                ScoreCounter.addReviveScore(500);

                if (BloodBootstrap.Instance.isEmitting)
                    BloodBootstrap.Instance.StopEmit();

                interactTime = timeToFinishInteraction;
                Debug.Log("Revive interaction finished!");
                if (humanAnim)
                    humanAnim.SabotagingDone(true);
                thisView.RPC("revivePlayer", RpcTarget.All, playerViewId);
                destroyThis(); // Can only be called inside interact

                if (GameManager.playerObj)
                {
                    CharTPController cc = GameManager.playerObj.GetComponent<CharTPController>();
                    cc.DisableKeyInput(false);
                    cc.DisableMouseInput(false);
                }
            }
        }
        else // Reset timer
        {
            if (interactedOnce)
            {
                interactedOnce = false;

                if (GameManager.playerObj)
                {
                    CharTPController cc = GameManager.playerObj.GetComponent<CharTPController>();
                    cc.DisableKeyInput(false);
                    cc.DisableMouseInput(false);
                }
                if (humanAnim)
                    humanAnim.SabotagingDone(false);

                if (BloodBootstrap.Instance.isEmitting)
                    thisView.RPC("playEcsVfx", RpcTarget.All, false);
            }

            //if (vfx.isEmitting)
            //    vfx.Stop();


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
        //might not have thisView yet
        PhotonView.Get(this).RPC("setPlayerID", RpcTarget.All, _viewId);
        timeoutCounter = 0;
    }

    [PunRPC]
    private void setPlayerID(int _viewId)
    {        
        playerViewId = _viewId;
        Debug.LogFormat("Setting ID, {0}", playerViewId);
    }

    [PunRPC]
    private void revivePlayer(int viewID)
    {
        //respawn player
        CharTPController target = null;
        foreach (var p in CharTPController.PlayerControllerRefs)
        {
            if (viewID == p.controller.photonView.ViewID)
                target = p.controller;
        }
        Debug.LogFormat("Recieved ID, {0}", viewID);
        if (target)
        { 
            target.GetComponent<CharHealth>().Respawn(1);
            Debug.LogFormat("Respawned {0}", target.name);
        }
        else
            Debug.Log("cant find player");
    }

    [PunRPC]
    private void playEcsVfx(bool b)
    {
        if (b)
        {
            BloodBootstrap.Instance.SetEmitterSource(transform.position, transform.forward);
            BloodBootstrap.Instance.Emit();
        }
        else 
        { 
            BloodBootstrap.Instance.StopEmit();
        }
    }

    [PunRPC]
    private void resetTimeoutTimer()
    {
        timeoutCounter = 0;
    }
}
