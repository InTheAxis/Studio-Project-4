using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScreenControllerTemp : Singleton<HUDScreenControllerTemp>
{
    [SerializeField]
    private GameObject deadScreen;
    [SerializeField]
    private GameObject endScreen;

    private void Start()
    {
        deadScreen.SetActive(false);
        endScreen.SetActive(false);
    }

    private void OnEnable()
    {
        SingletonPun<WinLose>.instance.winLossCallback += winLoss;
        CharHealth playerHealthComp = GameManager.playerObj.GetComponent<CharHealth>();
        playerHealthComp.OnDead += playerDied;
        playerHealthComp.OnRespawn += playerRespawned;
    }
    private void OnDisable()
    {
        SingletonPun<WinLose>.instance.winLossCallback -= winLoss;
        CharHealth playerHealthComp = GameManager.playerObj.GetComponent<CharHealth>();
        playerHealthComp.OnDead -= playerDied;
        playerHealthComp.OnRespawn -= playerRespawned;
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
}
