using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBGAudioController : AudioController
{
    private void Start()
    {
        SetMusic();
        Play("mainmenuBG");
    }
}