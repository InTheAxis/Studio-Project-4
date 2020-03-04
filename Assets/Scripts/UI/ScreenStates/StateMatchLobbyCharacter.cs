using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMatchLobbyCharacter : State
{
    [SerializeField]
    private GameObject playerContainerRef;

    public override string Name { get { return "LobbyCharacter"; } }

    public event Action<int, bool> eventModelSelect;


    public void selectCharacter(GameObject go)
    {
        Debug.Log("Selected " + go.name);

        int modelIndex = -1;
        switch (go.name)
        {
            case "Monster":
                modelIndex = 0;
                break;
            case "Male":
                modelIndex = 1;
                break;
            case "Female":
                modelIndex = 2;
                break;
        }

        if (modelIndex >= 0)
        {
            NetworkClient.setPlayerProperty("charModel", modelIndex);
            ScreenStateHelperNetwork.instance.sendModelChange(modelIndex);
            setModel(0, modelIndex);
            eventModelSelect?.Invoke(modelIndex, true);
        }
    }

    public void setModel(int playerIndex, int modelIndex)
    {
        Transform playerTransform = playerContainerRef.transform.GetChild(playerIndex);
        for (int i = 0; i < 3; ++i)
            playerTransform.GetChild(i).gameObject.SetActive(i == modelIndex);
    }

    public void Cancel()
    {
        StateController.Hide(Name);
    }

    public override void onShow()
    {
        base.onShow();
        StartCoroutine(StateController.fadeCanvasGroup(GetComponent<CanvasGroup>(), true, 10.0f));
    }
}
