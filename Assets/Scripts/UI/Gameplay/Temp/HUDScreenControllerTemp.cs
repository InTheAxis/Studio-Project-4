using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScreenControllerTemp : Singleton<HUDScreenControllerTemp>
{
    [SerializeField]
    private GameObject deadScreen;
    [SerializeField]
    private GameObject endScreen;

    private IEnumerator registrationCour = null;

    private void Start()
    {
        deadScreen.SetActive(false);
        endScreen.SetActive(false);

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
        registrationCour = deregisterCallbacks();
        StartCoroutine(registrationCour);
    }

    private void playerRespawned()
    {
        deadScreen.SetActive(false);
    }
    private void playerDied()
    {
        deadScreen.SetActive(true);
    }

    private void winLoss(bool isHunterWin)
    {
        endScreen.SetActive(true);
    }

    private IEnumerator registerCallbacks()
    {
        while (WinLose.instance == null)
            yield return null;

        WinLose.instance.winLossCallback += winLoss;
        CharHealth playerHealthComp = GameManager.playerObj.GetComponent<CharHealth>();
        playerHealthComp.OnDead += playerDied;
        playerHealthComp.OnRespawn += playerRespawned;
    }
    private IEnumerator deregisterCallbacks()
    {
        while (WinLose.instance == null)
            yield return null;

        WinLose.instance.winLossCallback -= winLoss;
        CharHealth playerHealthComp = GameManager.playerObj.GetComponent<CharHealth>();
        playerHealthComp.OnDead -= playerDied;
        playerHealthComp.OnRespawn -= playerRespawned;
    }
}
