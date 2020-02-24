using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : AudioController
{
    public void Step()
    {
        Play("footstep_stone");
    }
}