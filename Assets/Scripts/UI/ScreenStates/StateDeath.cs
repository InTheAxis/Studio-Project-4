using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StateDeath : State
{
    [SerializeField]
    private Volume postProcessing = null;
    [SerializeField]
    private TextMeshProUGUI tmCountdown = null;

    private IEnumerator registrationCour = null;

    public override string Name { get { return "Death"; } }

    private ColorAdjustments adjustment = null;

    private void Start()
    {

    }

    private void Update()
    {
        
    }

    public void Spectate()
    {
        Debug.Log("Spectate");
        StateController.showNext("Spectate");
    }

    public void Disconnect()
    {
        Debug.Log("Disconnect");

        if (PhotonNetwork.IsMasterClient)
            GameManager.instance.sendForceGameEnd(false);
        StateMainmenu.isReturningFromGame = true;
        StateGameover.isGameover = false;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("NewMainMenu");
    }

    public override void onShow()
    {
        Cursor.lockState = CursorLockMode.None;
        postProcessing.profile.TryGet(out adjustment);
        adjustment.saturation.value = -100.0f;

        base.onShow();

        if (registrationCour != null)
            StopCoroutine(registrationCour);
        registrationCour = registerCallbacks();
        StartCoroutine(registrationCour);
    }

    public override void onHide()
    {
        adjustment.saturation.value = 0.0f;

        if (registrationCour != null)
            StopCoroutine(registrationCour);
        deregisterCallbacks();

        base.onHide();
    }

    private IEnumerator registerCallbacks()
    {
        while (WinLose.instance == null || GameManager.playerObj == null)
            yield return null;

        WinLose.instance.winLossCallback += winLoss;
        if (GameManager.playerObj != null)
        {
            CharHealth playerHealthComp = GameManager.playerObj.GetComponent<CharHealth>();
            playerHealthComp.OnRespawn += playerRespawned;
        }
        else
            Debug.LogError("No player object");
    }
    private void deregisterCallbacks()
    {
        if (WinLose.instance != null)
            WinLose.instance.winLossCallback -= winLoss;
        if (GameManager.playerObj != null)
        {
            CharHealth playerHealthComp = GameManager.playerObj.GetComponent<CharHealth>();
            playerHealthComp.OnRespawn -= playerRespawned;
        }
    }

    private void winLoss(bool isHunterWin)
    {
        /* To set to end screen */
        Debug.Log("New HUD Received WinLoss");
    }

    private void playerRespawned()
    {
        GameManager.setCamera(GameManager.playerObj);
        StateController.showNext("Gameplay");
    }
}
