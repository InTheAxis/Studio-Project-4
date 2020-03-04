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
                int preferred = PlayerPrefs.GetInt("settings-resindex");
                if (preferred >= 0 && preferred < resolutions.Length)
                    currentResIndex = preferred;

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
            masterVol = 0.25f;
            effectVol = 0.50f;
            musicVol = 0.30f;
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
        masterVol = value;
        PlayerPrefs.SetFloat("settings-mastervol", masterVol);
        audioMixer.SetFloat("MasterVolume", masterVol * 80.0f - 80.0f);
    }

    public void setEffectsVolume(float value)
    {
        effectVol = value;
        PlayerPrefs.SetFloat("settings-effectvol", effectVol);
        audioMixer.SetFloat("SFXVolume", effectVol * 80.0f - 80.0f);
    }

    public void setMusicVolume(float value)
    {
        musicVol = value;
        PlayerPrefs.SetFloat("settings-musicvol", musicVol);
        audioMixer.SetFloat("MusicVolume", musicVol * 80.0f - 80.0f);
    }

    public void applyCurrentSettings()
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        Screen.SetResolution(resolutions[currentResIndex].width, resolutions[currentResIndex].height, fullscreen);
        QualitySettings.vSyncCount = vSync ? 1 : 0;
        audioMixer.SetFloat("MasterVolume", masterVol * 80.0f - 80.0f);
        audioMixer.SetFloat("SFXVolume", effectVol * 80.0f - 80.0f);
        audioMixer.SetFloat("MusicVolume", musicVol * 80.0f - 80.0f);
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


    private float Remap(float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }

}
