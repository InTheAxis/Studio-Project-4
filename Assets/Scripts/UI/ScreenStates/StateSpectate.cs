using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class StateSpectate : State
{
    [SerializeField]
    private TextMeshProUGUI tmUsername = null;

    private IEnumerator registrationCour = null;

    public override string Name { get { return "Spectate"; } }

    public override void onShow()
    {
        setNextPlayerSpectate(1);

        base.onShow();

        if (registrationCour != null)
            StopCoroutine(registrationCour);
        registrationCour = registerCallbacks();
        StartCoroutine(registrationCour);
    }

    public override void onHide()
    {
        if (registrationCour != null)
            StopCoroutine(registrationCour);
        deregisterCallbacks();

        base.onHide();
    }

    public void selectPrevious()
    {
        setNextPlayerSpectate(-1);
    }

    public void selectNext()
    {
        setNextPlayerSpectate(1);
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

    private void setNextPlayerSpectate(int offset)
    {
        // +ve offset - Next Player
        // -ve offset - Prev Player

        // Edge case: Only one player is in game. Don't change the camera
        if (CharTPController.PlayerControllerRefs.Count == 1)
            return;
        // There is only one monster, and that's the host. If host, don't change camera
        if (PhotonNetwork.IsMasterClient)
            return;

        int currIndex = CharTPController.PlayerControllerRefs.FindIndex(obj => obj.controller == CharTPCamera.Instance.charControl);
        currIndex += offset;

        System.Func<int, bool> checkMonster = (int index) =>
        {
            if (CharTPController.PlayerControllerRefs[index].controller.GetComponent<MonsterEnergy>() == null)
                return true;

            if (offset > 0)
                ++currIndex;
            else
                --currIndex;
            return false;
        };

        do
        {
            while (currIndex < 0)
                currIndex += CharTPController.PlayerControllerRefs.Count;
            while (currIndex >= CharTPController.PlayerControllerRefs.Count)
                currIndex -= CharTPController.PlayerControllerRefs.Count;
        }
        while (!checkMonster(currIndex));

        GameManager.setCamera(CharTPController.PlayerControllerRefs[currIndex].controller);
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
