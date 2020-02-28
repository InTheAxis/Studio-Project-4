using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleAudioController : AudioController
{
    protected override void Awake()
    {
        // init audio
        Sound sound = new Sound();
        sound.clip = (AudioClip)Resources.Load("impact0", typeof(AudioClip));
        sound.pitch = 1;
        sound.volume = 0.5f;
        sounds.Add(sound);
        base.Awake();
    }

    public void DestructAudio()
    {
        Play("impact0");
    }
}