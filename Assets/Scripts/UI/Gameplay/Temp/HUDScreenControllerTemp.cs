using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScreenControllerTemp : Singleton<HUDScreenControllerTemp>
{
    [SerializeField]
    private GameObject deadScreen;

    [SerializeField]
    private GameObject spectateScreen;

    [SerializeField]
    private GameObject endScreen;

    private IEnumerator registrationCour = null;

    private void Start()
    {
        deadScreen.SetActive(false);
        endScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (deadScreen.activeSelf || spectateScreen.activeSelf))
            if (deadScreen.activeSelf)
            {
                deadScreen.SetActive(false);
                spectateScreen.SetActive(true);
                setNextPlayerSpectate();
            }
            else
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
        deadScreen.SetActive(false);
        spectateScreen.SetActive(false);
        GameManager.setCamera(GameManager.playerObj);
    }

    private void playerDied()
    {
        deadScreen.SetActive(true);
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
        endScreen.SetActive(true);
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