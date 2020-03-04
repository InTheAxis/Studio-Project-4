using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAudioController : AudioController
{
    private void Start()
    {
        SetSFX();
    }

    public void Charge()
    {
        Play("charge0");
    }

    public void Attack()
    {
        Play("charge0");
    }
}