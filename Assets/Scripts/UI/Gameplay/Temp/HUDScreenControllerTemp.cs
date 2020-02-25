using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScreenControllerTemp : Singleton<HUDScreenControllerTemp>
{
    [SerializeField]
    private GameObject endScreen;

    private void Start()
    {
        endScreen.SetActive(false);
    }

    private void OnEnable()
    {
        SingletonPun<WinLose>.instance.winLossCallback += winLoss;
    }
    private void OnDisable()
    {
        SingletonPun<WinLose>.instance.winLossCallback -= winLoss;
    }

    private void winLoss(bool isHunterWin)
    {
        endScreen.SetActive(true);
    }
}
