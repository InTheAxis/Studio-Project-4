using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScreenController : Singleton<HUDScreenController>
{
    private IEnumerator registrationCour = null;

    private void Update()
    {
        string currentStateName = StateController.getCurrentState().Name;

        if (Input.GetKeyDown(KeyCode.Space) && (currentStateName == "Dead" || currentStateName == "Spectate"))
            if (currentStateName == "Dead")
            {
                StateController.showNext("Spectate");
                setNextPlayerSpectate();
            }
            else // Spectating already
                setNextPlayerSpectate();
    }

    private void OnEnable()
    {
        if (registrationCour != null)
            StopCoroutine(registrationCour);
        registrationCour = registerCallbacks();
        StartCoroutine(registrationCour);
    }

    private void OnDisable()
    {
        if (registrationCour != null)
            StopCoroutine(registrationCour);
        //registrationCour = deregisterCallbacks();
        //StartCoroutine(registrationCour);

        WinLose.instance.winLossCallback -= winLoss;
        if (GameManager.playerObj != null)
        {
            CharHealth playerHealthComp = GameManager.playerObj.GetComponent<CharHealth>();
            if (playerHealthComp != null)
            {
                playerHealthComp.OnDead -= playerDied;
                playerHealthComp.OnRespawn -= playerRespawned;
            }
        }
    }

    private void playerRespawned()
    {
        StateController.showNext("Gameplay");
        GameManager.setCamera(GameManager.playerObj);
    }

    private void playerDied()
    {
        /* To change to Dead instead of Spectate */

        //StateController.showNext("Dead");
        StateController.showNext("Spectate");
    }

    private void setNextPlayerSpectate()
    {
        // Edge case: Only one player is in game. Don't change the camera
        if (CharTPController.PlayerControllerRefs.Count == 1)
            return;

        int currIndex = CharTPController.PlayerControllerRefs.FindIndex(obj => obj.controller == CharTPCamera.Instance.charControl);
        if (++currIndex == CharTPController.PlayerControllerRefs.Count)
            currIndex = 0;
        GameManager.setCamera(CharTPController.PlayerControllerRefs[currIndex].controller);
    }

    private void winLoss(bool isHunterWin)
    {
        /* To set to end screen */
        Debug.Log("New HUD Received WinLoss");
    }

    private IEnumerator registerCallbacks()
    {
        while (WinLose.instance == null || GameManager.playerObj == null)
            yield return null;

        WinLose.instance.winLossCallback += winLoss;
        if (GameManager.playerObj != null)
        {
            CharHealth playerHealthComp = GameManager.playerObj.GetComponent<CharHealth>();
            playerHealthComp.OnDead += playerDied;
            playerHealthComp.OnRespawn += playerRespawned;
        }
        else
        {
            Debug.LogWarning("No player object");
        }
    }

    private IEnumerator deregisterCallbacks()
    {
        while (WinLose.instance == null || GameManager.playerObj == null)
            yield return null;

        WinLose.instance.winLossCallback -= winLoss;
        CharHealth playerHealthComp = GameManager.playerObj.GetComponent<CharHealth>();
        playerHealthComp.OnDead -= playerDied;
        playerHealthComp.OnRespawn -= playerRespawned;
    }
}