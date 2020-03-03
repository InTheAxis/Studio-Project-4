using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameplaySettings : DoNotDestroySingleton<GameplaySettings>
{
    [SerializeField]
    private AudioMixer audioMixer = null;

    public bool vSync = false;
    public float masterVol = 1.0f;
    public float effectVol = 1.0f;
    public float musicVol = 1.0f;

    public List<string> options;
    public int currentResIndex = 0;
    public int qualityIndex = 0;
    public bool fullscreen = false;

    private Resolution[] resolutions = null;

    private void Start()
    {

        if(resolutions == null)
        {
            resolutions = Screen.resolutions;
            options = new List<string>();

            for (int i = 0; i < resolutions.Length; ++i)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "hz";
                options.Add(option);

                /* Find current resolution's index */
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                    currentResIndex = i;
            }

            if (PlayerPrefs.HasKey("settings-resindex"))
            {
                currentResIndex = PlayerPrefs.GetInt("settings-resindex");
                Debug.Log("Found preferred settings: " + currentResIndex);
            }
            else
            {
                Debug.Log("No preferred resolution previously");
            }

        }

        if(PlayerPrefs.HasKey("settings-vsync"))
        {
            vSync = PlayerPrefs.GetInt("settings-vsync") >= 1;
            masterVol = PlayerPrefs.GetFloat("settings-mastervol");
            effectVol = PlayerPrefs.GetFloat("settings-effectvol");
            musicVol = PlayerPrefs.GetFloat("settings-musicvol");
            qualityIndex = PlayerPrefs.GetInt("settings-qualityindex", qualityIndex);
            fullscreen = PlayerPrefs.GetInt("settings-fullscreen") >= 1;
        }
        else
        {
            vSync = QualitySettings.vSyncCount >= 1;
            masterVol = 1.0f;
            effectVol = 1.0f;
            musicVol = 1.0f;
            qualityIndex = QualitySettings.GetQualityLevel();
            fullscreen = Screen.fullScreen;
        }

        applyCurrentSettings();
    }

    public void setQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        qualityIndex = index;
        PlayerPrefs.SetInt("settings-qualityindex", qualityIndex);
    }

    public void setFullscreen(int index)
    {
        fullscreen = index >= 1;
        Screen.fullScreen = fullscreen;
        PlayerPrefs.SetInt("settings-fullscreen", fullscreen ? 1 : 0);
    }

    public void setResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
        currentResIndex = index;
        PlayerPrefs.SetInt("settings-resindex", currentResIndex);
    }

    public void setVSync(bool state)
    {
        QualitySettings.vSyncCount = state ? 1 : 0;
        vSync = state;
        PlayerPrefs.SetInt("settings-vsync", vSync ? 1 : 0);
    }

    public void setMasterVolume(float value)
    {
        audioMixer.SetFloat("MasterVolume", value.Remap(0.0f, 1.0f, -80.0f, 0.0f));
        masterVol = value;
        PlayerPrefs.SetFloat("settings-mastervol", masterVol);
    }

    public void setEffectsVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", value.Remap(0.0f, 1.0f, -80.0f, 0.0f));
        effectVol = value;
        PlayerPrefs.SetFloat("settings-effectvol", effectVol);
    }

    public void setMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", value.Remap(0.0f, 1.0f, -80.0f, 0.0f));
        musicVol = value;
        PlayerPrefs.SetFloat("settings-musicvol", musicVol);
    }

    public void applyCurrentSettings()
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Screen.SetResolution(resolutions[currentResIndex].width, resolutions[currentResIndex].height, fullscreen);
        QualitySettings.vSyncCount = vSync ? 1 : 0;
        audioMixer.SetFloat("MasterVolume", masterVol.Remap(0.0f, 1.0f, -80.0f, 0.0f));
        audioMixer.SetFloat("SFXVolume", effectVol.Remap(0.0f, 1.0f, -80.0f, 0.0f));
        audioMixer.SetFloat("MusicVolume", musicVol.Remap(0.0f, 1.0f, -80.0f, 0.0f));
    }

    public void saveAll()
    {
        PlayerPrefs.SetInt("settings-vsync", vSync ? 1 : 0);
        PlayerPrefs.SetInt("settings-fullscreen", fullscreen ? 1 : 0);
        PlayerPrefs.SetFloat("settings-mastervol", masterVol);
        PlayerPrefs.SetFloat("settings-effectvol", effectVol);
        PlayerPrefs.SetFloat("settings-musicvol", musicVol);
        PlayerPrefs.SetInt("settings-resindex", currentResIndex);
        PlayerPrefs.SetInt("settings-qualityindex", qualityIndex);
    }

    private void OnApplicationQuit()
    {
        saveAll();
    }


}
