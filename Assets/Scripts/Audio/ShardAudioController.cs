using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardAudioController : AudioController
{
    private float audioBuffer = 1;
    private float lastAudio = 0;

    protected override void Awake()
    {
        base.Awake();
        // init audio
        Sound sound = new Sound();
        sound.clip = (AudioClip)Resources.Load("impact0", typeof(AudioClip));
        sound.pitch = 0.8f;
        sound.volume = 0.2f;
        sounds.Add(sound);
    }

    public void CollideAudio()
    {
        if ((Time.time - lastAudio) > audioBuffer)
        {
            Play("impact0");
            lastAudio = Time.time;
        }
        else
            return;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollideAudio();
    }
}