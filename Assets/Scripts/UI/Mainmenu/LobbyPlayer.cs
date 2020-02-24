using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyPlayer
{
    public GameObject model;
    public TextMeshPro tmName;
    public GameObject tick;
    public string name;

    public LobbyPlayer(GameObject model)
    {
        setModel(model);
    }

    public void setModel(GameObject model)
    {
        this.model = model;
        tmName = model.transform.Find("Name").GetComponent<TextMeshPro>();
        tmName.text = "";
        tick = model.transform.Find("Tick").gameObject;
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
