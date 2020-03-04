using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class InteractableTower : InteractableBase
{
    [SerializeField]
    private float timeToFinishInteractionMonster = 2.0f;
    [SerializeField]
    private float timeToFinishInteractionHuman = 2.0f;
    private float timeToFinishInteraction = 2.0f;

    private bool wasInteracting = false;
    private float interactTime = 0.0f;

    private bool interactedOnce = false;
    // In the process of being destroyed through Destructible script. Don't interact anymore once set
    private bool isDestroyed = false;

    [Header("VFX")]
    [SerializeField]
    private ParticleSystem sparks = null;


    [Header("Tower Light Indicators")]
    [SerializeField]
    private GameObject beingInteractedWithHunterLight = null;
    [SerializeField]
    private GameObject beingInteractedWithSurvivorLight = null;
    [SerializeField]
    private List<GameObject> interactStagesLights = new List<GameObject>();
    private int hunterLightIndex;
    private int survivorLightIndex;

    private int currLight = -1;
    private int currStage = 0;
    // Used to discard old light change requests
    private int lightTimestamp = 0;

    private PhotonView thisView;

    private HumanAnimationSM humanAnim = null;
    private MonsterAnimationSM monsterAnim = null;

    private void Start()
    {
        thisView = PhotonView.Get(this);

        if (Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            timeToFinishInteraction = timeToFinishInteractionMonster;
        }
        else
        {
            timeToFinishInteraction = timeToFinishInteractionHuman;
        }

        hunterLightIndex = interactStagesLights.Count;
        survivorLightIndex = hunterLightIndex + 1;
        // Turn on the first stage light
        turnOnLight(currStage, currStage);
    }
    public override string getInteractableName() { return "Tower"; }

    public override void interact()
    {
        if (isDestroyed)
            return;

        wasInteracting = true;
        Debug.Log("Interact");

        if(!sparks.isEmitting)
            thisView.RPC("playVFX", RpcTarget.All, true);
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
                GameManager.playerObj.GetComponent<CharTPController>().DisableKeyInput(true);
            }

            if (humanAnim)
                humanAnim.IsSabotaging();
            if (monsterAnim)
                monsterAnim.IsSabotaging();

            if (interactTime >= timeToFinishInteraction) // Done interacting
            {
                interactTime = timeToFinishInteraction;
                Debug.Log("Tower interaction finished!");
                HumanUnlockTool unlock = GameManager.playerObj.GetComponent<HumanUnlockTool>();
                MonsterEnergy recharge = GameManager.playerObj.GetComponent<MonsterEnergy>();
                if (unlock) // Is Human
                {
                    // Advance stage
                    if (++currStage >= interactStagesLights.Count) // Finished all stages. Destroy
                    {
                        isDestroyed = true;

                        unlock.Unlock(HumanUnlockTool.TYPE.RANDOM);
                        if (humanAnim)
                            humanAnim.SabotagingDone(true);

                        Destructible thisDestuctibleComp = GetComponent<Destructible>();
                        if (thisDestuctibleComp != null)
                            thisDestuctibleComp.Destruct(null);
                        else
                            destroyThis(); // Can only be called inside interact
                    }
                    else // Has more stages. Go to next stage
                    {
                        turnOnLight(currStage, currStage);
                        interactTime = 0.0f;
                    }
                }
                else if (recharge) // Is Monster
                {
                    if (monsterAnim)
                        monsterAnim.SabotagingDone(true);
                    recharge.RechargePercent(interactTime / timeToFinishInteraction);
                    interactDone = true;
                    Debug.Log("Recharged");
                }
            }
            else // Not done interacting
            {
                // Activate being interacted lights
                if (PhotonNetwork.IsMasterClient)
                    turnOnLight(hunterLightIndex, currStage);
                else
                    turnOnLight(survivorLightIndex, currStage);
            }
        }
        else // Reset timer
        {
            if (interactedOnce)
            {

                if (sparks.isEmitting)
                    thisView.RPC("playVFX", RpcTarget.All, false);

                interactedOnce = false;

                if (GameManager.playerObj)
                {
                    GameManager.playerObj.GetComponent<CharTPController>().DisableKeyInput(false);
                }
                if (humanAnim)
                    humanAnim.SabotagingDone(false);
                if (monsterAnim)
                    monsterAnim.SabotagingDone(false);
            }

            // Stopped interacting. Set light back to current stage
            if (interactTime > 0.0f)
                turnOnLight(currStage, currStage);

            interactTime = 0.0f;
            interactDone = false;
        }
    }

    public override string getUncarriedTooltip()
    {
        if (Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            if (wasInteracting)
                return "Recharging " + Mathf.RoundToInt(interactTime / timeToFinishInteraction * 100.0f) + "%";
            else
                return base.getUncarriedTooltip() + "Recharge Energy from Tower";
        }
        else
        {
            if (wasInteracting)
                return "Destroying " + Mathf.RoundToInt(interactTime / timeToFinishInteraction * 100.0f) + "%";
            else
                return base.getUncarriedTooltip() + "Destroy Tower";
        }
    }

    private void turnOnLight(int stage, int nowStage)
    {
        // Don't change light if current active light is same as new requested light
        if (stage == currLight)
            return;

        GameObject activeLight = getLight();
        if (activeLight != null)
            activeLight.SetActive(false);

        currStage = nowStage;
        currLight = stage;

        if (stage <= survivorLightIndex)
        {
            thisView.RPC("setLight", RpcTarget.Others, stage, nowStage);

            activeLight = getLight(stage);
            activeLight.SetActive(true);
        }
        else
            Debug.LogError("Attempted to turn on Light with invalid index " + stage + " for Tower with ViewID of " + thisView?.ViewID);
    }

    [PunRPC]
    private void setLight(int index, int nowStage, PhotonMessageInfo messageInfo)
    {
        // Ignore requests older than the newest request that we've received
        if (messageInfo.SentServerTimestamp < lightTimestamp)
            return;
        lightTimestamp = messageInfo.SentServerTimestamp;

        // var index
        // Legend:
        // n = No. of Stage Lights
        //
        // 0 to n - 1 == Index of light in interactStagesLights
        // n          == beingInteractedWithHunterLight
        // n + 1      == beingInteractedWithSurvivorLight

        Debug.Log("Received request to change light to Index " + index + " for Tower with ViewID " + thisView?.ViewID);

        GameObject activeLight = getLight();
        if (activeLight != null)
            activeLight.SetActive(false);

        currStage = nowStage;
        currLight = index;

        activeLight = getLight(index);
        if (activeLight != null)
            activeLight.SetActive(true);
    }

    private GameObject getLight()
    {
        return getLight(currLight);
    }
    private GameObject getLight(int index)
    {
        int count = interactStagesLights.Count;
        if (index < 0)
            return null;
        else if (index < count)
            return interactStagesLights[index];
        else if (index == count)
            return beingInteractedWithHunterLight;
        else if (index == count + 1)
            return beingInteractedWithSurvivorLight;
        else
            return null;
    }

    [PunRPC]
    private void playVFX(bool b)
    {
        if (b && !sparks.isEmitting)
            sparks.Play();
        else if (!b && sparks.isEmitting)
            sparks.Stop();
    }
}
