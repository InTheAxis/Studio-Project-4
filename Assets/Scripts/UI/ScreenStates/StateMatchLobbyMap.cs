using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StateMatchLobbyMap : State
{
    [Header("References")]
    [SerializeField]
    private Sprite[] mapImages = null;
    [SerializeField]
    private string[] mapTitles = null;
    [SerializeField]
    private Image mapDisplayImage = null;
    [SerializeField]
    private TextMeshProUGUI tmMapTitle = null;

    [SerializeField]
    private GameObject previous = null;
    [SerializeField]
    private GameObject next = null;

    public override string Name { get { return "LobbyMap"; } }

    public static int currentMapIndex { get; private set; } = 0;

    public void selectPrevious()
    {
        currentMapIndex = Mathf.Clamp(currentMapIndex - 1, 0, mapImages.Length - 1);
        previous.SetActive(currentMapIndex > 0);
        next.SetActive(true);
        selectMap();
    }

    public void selectNext()
    {
        currentMapIndex = Mathf.Clamp(currentMapIndex + 1, 0, mapImages.Length - 1);
        previous.SetActive(true);
        next.SetActive(currentMapIndex < mapImages.Length - 1);
        selectMap();
    }

    private void selectMap()
    {
        mapDisplayImage.sprite = mapImages[currentMapIndex];
        tmMapTitle.text = mapTitles[currentMapIndex];

        MapSelector.SelectMap((MapSelector.Maps)currentMapIndex);
    }

    public void Cancel()
    {
        StateController.Hide(Name);
    }

    public override void onShow()
    {
        currentMapIndex = 0;
        selectMap();
        base.onShow();
    }
}
