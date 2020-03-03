using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioInstance : MonoBehaviour
{
    public static AudioInstance instance { get; private set; }

    [SerializeField]
    public AudioMixerGroup sfxMixer;

    [SerializeField]
    public AudioMixerGroup musicMixer;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }
}