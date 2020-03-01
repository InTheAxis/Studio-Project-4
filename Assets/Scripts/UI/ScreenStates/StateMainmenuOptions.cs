using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Linq;

public class StateMainmenuOptions : State
{
    [Header("Navigation")]
    [SerializeField]
    [Tooltip("The indicator that shows which sub-menu is showing.")]
    private RectTransform indicator = null;
    [SerializeField]
    [Tooltip("The transform that holds all the primary Mainmenu buttons.")]
    private RectTransform buttonHolder = null;
    [SerializeField]
    [Tooltip("The transform that holds all the sub-menus.")]
    private RectTransform subMenuHolder = null;

    [Header("Graphics")]
    [SerializeField]
    private TMP_Dropdown tmQuality = null;
    [SerializeField]
    private TMP_Dropdown tmFullscreen = null;
    [SerializeField]
    private TMP_Dropdown tmResolution = null;
    [SerializeField]
    private Image vSyncOn = null;
    [SerializeField]
    private Image vSyncOff = null;

    [Header("Sound")]
    [SerializeField]
    private AudioMixer audioMixer = null;
    [SerializeField]
    private TextMeshProUGUI tmMasterVolume = null;
    [SerializeField]
    private TextMeshProUGUI tmEffectVolume = null;
    [SerializeField]
    private TextMeshProUGUI tmMusicVolume = null;

    private Resolution[] resolutions = null;


    public override string Name { get { return "MainmenuOptions"; } }

    private IEnumerator iSlideIndicator = null;
    private GameObject currentSubMenu = null;

    #region Navigation

    /* Slides the indicator up and down based on the nav button pressed */
    private void selectNavButton(GameObject go)
    {
        indicator.gameObject.SetActive(true);
        if (iSlideIndicator != null)
            StopCoroutine(iSlideIndicator);
        iSlideIndicator = slideIndicator(go.GetComponent<RectTransform>().anchoredPosition.x);
        StartCoroutine(iSlideIndicator);
    }

    /* Animate the sliding of the indicator in the X axis */
    private IEnumerator slideIndicator(float targetX)
    {
        Vector2 anchoredPos = indicator.anchoredPosition;
        while (Mathf.Abs(targetX - anchoredPos.x) > 0.005f)
        {
            anchoredPos.x = Mathf.Lerp(anchoredPos.x, targetX, Time.deltaTime * 12.0f);
            indicator.anchoredPosition = anchoredPos;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        indicator.anchoredPosition = anchoredPos;
    }

    /* Toggles on/off the sub-menu for a particular nav button */
    public void toggleSubMenu(GameObject go)
    {
        selectNavButton(go);
        /* Find target sub menu */
        GameObject nextMenu = subMenuHolder.Find(go.name).gameObject;

        /* Hide current sub menu if I pressed on another item's sub menu */
        if (currentSubMenu != null && currentSubMenu != nextMenu)
            currentSubMenu.SetActive(false);

        /* Update */
        currentSubMenu = nextMenu;
        currentSubMenu.SetActive(!currentSubMenu.activeSelf);
    }

    #endregion

    #region Graphics Settings

    public void setQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }

    public void setFullscreen(int index)
    {
        Screen.fullScreen = index != 0;
    }

    public void setResolution(int index)
    {
        Screen.SetResolution(resolutions[index].width, resolutions[index].height, Screen.fullScreen);
    }

    public void setVSync(bool state)
    {
        if(state)
        {
            vSyncOn.color = ThemeColors.positive;
            vSyncOff.color = ThemeColors.negative;
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            vSyncOn.color = ThemeColors.negative;
            vSyncOff.color = ThemeColors.positive;
            QualitySettings.vSyncCount = 0;
       
        }
    }

    #endregion

    #region Sound Settings

    public void onMasterVolumeChange(float value)
    {
        tmMasterVolume.text = ((int)(value * 100.0f)).ToString() + "%";
        audioMixer.SetFloat("MasterVolume", value.Remap(0.0f, 1.0f, -80.0f, 0.0f));
    }


    public void onEffectVolumeChange(float value)
    {
        tmEffectVolume.text = ((int)(value * 100.0f)).ToString() + "%";
        audioMixer.SetFloat("SFXVolume", value.Remap(0.0f, 1.0f, -80.0f, 0.0f));
    }


    public void onMusicVolumeChange(float value)
    {
        tmMusicVolume.text = ((int)(value * 100.0f)).ToString() + "%";
        audioMixer.SetFloat("MusicVolume", value.Remap(0.0f, 1.0f, -80.0f, 0.0f));
    }

    #endregion


    public override void onShow()
    {
        /* Reset sub menu states */
        currentSubMenu = subMenuHolder.GetChild(0).gameObject;
        currentSubMenu.SetActive(true);
        for (int i = 1; i < subMenuHolder.childCount; ++i)
            subMenuHolder.GetChild(i).gameObject.SetActive(false);

        /* Populate supported video resolutions */
        if (resolutions == null)
        {
            resolutions = Screen.resolutions;
            tmResolution.ClearOptions();
            List<string> options = new List<string>();

            int currentIndex = 0;
            for (int i = 0; i < resolutions.Length; ++i)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "hz";
                options.Add(option);

                /* Find current resolution's index */
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                    currentIndex = i;
            }
            tmResolution.AddOptions(options);
            tmResolution.value = currentIndex;
            tmResolution.RefreshShownValue();
        }

        /* Update fullscreen status */
        tmFullscreen.value = Screen.fullScreen ? 1 : 0;
        tmFullscreen.RefreshShownValue();

        /* Update quality settings */
        tmQuality.value = QualitySettings.GetQualityLevel();
        tmQuality.RefreshShownValue();

        /* Update V-Sync Status */
        setVSync(QualitySettings.vSyncCount != 0);

        tmMasterVolume.text = "100%";
        tmEffectVolume.text = "100%";
        tmMusicVolume.text = "100%";


        base.onShow();
    }
}
