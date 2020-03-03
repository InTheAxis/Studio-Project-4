using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateGameplay : State
{
    [SerializeField]
    private GameObject hud = null;

    private IEnumerator registrationCour = null;

    public override string Name { get { return "Gameplay"; } }

    private void Start()
    {
        StateController.showNext(Name);
    }
    private void OnDestroy()
    {
        StateController.Unregister(this);
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            StateController.showNext("GamePause");
    }

    public override void onShow()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (GameManager.playerObj != null)
        {
            GameManager.playerObj.GetComponent<CharTPController>().disableKeyInput = false;
            GameManager.playerObj.GetComponent<CharTPController>().disableMouseInput = false;
            GameManager.playerObj.GetComponent<CharTPController>().disableMovement = false;
        }
        hud.SetActive(true);

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

    private IEnumerator registerCallbacks()
    {
        while (WinLose.instance == null || GameManager.playerObj == null)
            yield return null;

        WinLose.instance.winLossCallback += winLoss;
        if (GameManager.playerObj != null)
        {
            CharHealth playerHealthComp = GameManager.playerObj.GetComponent<CharHealth>();
            playerHealthComp.OnDead += playerDied;
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
            playerHealthComp.OnDead -= playerDied;
        }
    }

    private void playerDied()
    {
        Debug.Log("Gameplay received player death");
        StateController.showNext("Death");
    }

    private void winLoss(bool isHunterWin)
    {
        /* To set to end screen */
        Debug.Log("New HUD Received WinLoss");
    }
}
