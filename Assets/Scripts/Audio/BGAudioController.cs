using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGAudioController : AudioController
{
    public static BGAudioController instance;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }
}