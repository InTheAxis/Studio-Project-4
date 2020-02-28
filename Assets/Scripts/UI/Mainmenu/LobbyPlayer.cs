using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyPlayer
{
    public GameObject model;
    public TextMeshProUGUI tmName;
    public GameObject tick;
    public string name;

    public LobbyPlayer(GameObject parent)
    {
        setModel(parent);
    }

    public void setModel(GameObject parent)
    {
        model = parent.transform.Find("Model").gameObject;
        tmName = parent.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        tick = parent.transform.Find("Tick").gameObject;
        tmName.text = "";
        tick.SetActive(false);
    }

    public void setName(string name)
    {
        this.name = name;
        tmName.text = name;
    }

    public void setActive(string name)
    {
        model.SetActive(true);
        this.name = name;
        tmName.text = name;
    }

    public void setInactive()
    {
        model.SetActive(false);
        this.name = "";
        tmName.text = "";
    }

    public void toggleReady()
    {
        tick.SetActive(!tick.activeSelf);
    }


    public void setReady(bool state)
    {
        tick.SetActive(state);
    }

}
