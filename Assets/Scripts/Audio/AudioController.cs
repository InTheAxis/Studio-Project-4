using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;

    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        if (source == null)
            Debug.LogError("No audio source found on: " + gameObject.name);
    }

    public void Play(string name, float timeOffset = 0.0f)
    {
        if (source == null)
            return;
        AudioClip s = Array.Find(clips, clip => clip.name == name);
        if (s == null)
        {
            Debug.LogError("[Audio] " + name + " clip does not exist!");
            return;
        }
        source.clip = s;
        source.time = timeOffset;
        source.Play();
    }
}