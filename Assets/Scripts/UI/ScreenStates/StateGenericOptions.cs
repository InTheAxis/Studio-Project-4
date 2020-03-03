using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Linq;

public abstract class StateGenericOptions : State
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
    private TextMeshProUGUI tmMasterVolume = null;
    [SerializeField]
    private TextMeshProUGUI tmEffectVolume = null;
    [SerializeField]
    private TextMeshProUGUI tmMusicVolume = null;



    private IEnumerator iSlideIndicator = null;
    private GameObject currentSubMenu = null;

    private GameplaySettings settings = null;

    private void Awake()
    {
        StateController.Register(this);
        settings = FindObjectOfType<GameplaySettings>();

        if (settings == null)
            Debug.Log("Could not find GameplaySettings!");
    }

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
        settings.setQuality(index);
    }

    public void setFullscreen(int index)
    {
        settings.setFullscreen(index);
    }

    public void setResolution(int index)
    {
        settings.setResolution(index);
    }

    public void setVSync(bool state)
    {
        if (state)
        {
            vSyncOn.color = ThemeColors.positive;
            vSyncOff.color = ThemeColors.negative;
        }
        else
        {
            vSyncOn.color = ThemeColors.negative;
            vSyncOff.color = ThemeColors.positive;
        }

        settings.setVSync(state);
    }

    #endregion

    #region Sound Settings

    public void onMasterVolumeChange(float value)
    {
        tmMasterVolume.text = ((int)(value * 100.0f)).ToString() + "%";
        settings.setMasterVolume(value);
    }


    public void onEffectVolumeChange(float value)
    {
        tmEffectVolume.text = ((int)(value * 100.0f)).ToString() + "%";
        settings.setEffectsVolume(value);
    }


    public void onMusicVolumeChange(float value)
    {
        tmMusicVolume.text = ((int)(value * 100.0f)).ToString() + "%";
        settings.setMusicVolume(value);
    }

    #endregion

    public override void onShow()
    {
        /* Reset sub menu states */
        currentSubMenu = subMenuHolder.GetChild(0).gameObject;
        currentSubMenu.SetActive(true);
        for (int i = 1; i < subMenuHolder.childCount; ++i)
            subMenuHolder.GetChild(i).gameObject.SetActive(false);

        tmResolution.ClearOptions();
        tmResolution.AddOptions(settings.options);
        tmResolution.value = settings.currentResIndex;
        tmResolution.RefreshShownValue();

        /* Update fullscreen status */
        tmFullscreen.value = settings.fullscreen ? 1 : 0;
        tmFullscreen.RefreshShownValue();

        /* Update quality settings */
        tmQuality.value = settings.qualityIndex;
        tmQuality.RefreshShownValue();

        /* Update V-Sync Status */
        setVSync(settings.vSync);

        tmMasterVolume.text = ((int)settings.masterVol).ToString() + "%";
        tmEffectVolume.text = ((int)settings.effectVol).ToString() + "%";
        tmMusicVolume.text = ((int)settings.musicVol).ToString() + "%";

        base.onShow();
    }
}



