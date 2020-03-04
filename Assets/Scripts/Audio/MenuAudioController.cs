using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudioController : AudioController
{
    private bool ready = false;

    private void Start()
    {
        SetSFX();
    }

    public void Click()
    {
        Play("click0");
    }

    public void ToggleReady()
    {
        if (!ready)
            Ready();
        else
            NotReady();
        ready = !ready;
    }

    private void Ready()
    {
        Play("ready0");
    }

    private void NotReady()
    {
        Play("notready");
    }
}