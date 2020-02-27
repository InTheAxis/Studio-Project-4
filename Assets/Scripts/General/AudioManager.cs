using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private List<Sound> sfxSounds;
    private List<Sound> musicSounds;

    [SerializeField]
    private float sfxAudioMultiplier = 1.0f;

    [SerializeField]
    private float musicAudioMultiplier = 1.0f;

    public enum AudioState
    {
        Play,
        Pause,
        Resume,
        Stop,
    }

    private void Awake()
    {
        sfxSounds = new List<Sound>();
        musicSounds = new List<Sound>();

        if (sounds.Length > 0)
        {
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;

                if (s.sfx)
                    sfxSounds.Add(s);
                else
                    musicSounds.Add(s);

                if (s.playOnAwake)
                    setAudio(s.clip.name, AudioState.Play);
            }
        }
        Debug.Log("[Audio] Initialized!");
    }

    public void setAudio(string name, AudioState state)
    {
        Sound s = Array.Find(sounds, sound => sound.clip.name == name);
        if (s == null)
        {
            Debug.LogError("[Audio] " + name + " sound does not exist!");
            return;
        }

        switch (state)
        {
            case AudioState.Play:
                s.source.Play();
                break;

            case AudioState.Pause:
                s.source.Pause();
                break;

            case AudioState.Resume:
                s.source.UnPause();
                break;

            case AudioState.Stop:
                s.source.Stop();
                break;
        }
    }

    public void setSFXMultiplier(float multiplier)
    {
        sfxAudioMultiplier = multiplier;
        foreach (Sound s in sfxSounds)
            s.source.volume = s.volume * sfxAudioMultiplier;
    }

    public void setMusicMultiplier(float multiplier)
    {
        musicAudioMultiplier = multiplier;
        foreach (Sound s in musicSounds)
            s.source.volume = s.volume * musicAudioMultiplier;
    }
}