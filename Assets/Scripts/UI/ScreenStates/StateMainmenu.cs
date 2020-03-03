using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StateMainmenu : State
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

    public override string Name { get { return "Mainmenu"; } }

    private IEnumerator iSlideIndicator = null;
    private GameObject currentSubMenu = null;

    public static bool isReturningFromGame = false;

    private void Awake()
    {
        StateController.Register(this);
        if (isReturningFromGame)
        {
            isReturningFromGame = false;

            StateController.showNext(Name);
        }
    }
    private void OnDestroy()
    {
        StateController.Unregister(this);
    }

    public void Logout()
    {
        PlayerSettings.playerName = null;
        StateController.showPrevious();
    }

    /* Slides the indicator up and down based on the nav button pressed */
    private void selectNavButton(GameObject go)
    {
        indicator.gameObject.SetActive(true);
        if (iSlideIndicator != null)
            StopCoroutine(iSlideIndicator);
        iSlideIndicator = slideIndicator(go.GetComponent<RectTransform>().anchoredPosition.y);
        StartCoroutine(iSlideIndicator);
    }

    /* Toggles on/off the sub-menu for a particular nav button */
    public void toggleSubMenu(GameObject go)
    {
        selectNavButton(go);
        /* Find target sub menu */
        GameObject nextMenu = subMenuHolder.Find(go.name).gameObject;

        /* Hide current sub menu if I pressed on another item's sub menu */
        if (currentSubMenu != null && currentSubMenu != nextMenu)
            StateController.Hide(currentSubMenu.name);

        /* Update */
        currentSubMenu = nextMenu;
        if (!currentSubMenu.activeSelf)
            StateController.Show(go.name);
        else
            StateController.Hide(go.name);
    }

    public void toggleObject(GameObject go)
    {
        go.SetActive(!go.activeSelf);
        if(go.activeSelf)
            StartCoroutine(StateController.fadeCanvasGroup(go.GetComponent<CanvasGroup>(), true, 10.0f));
    }

    /* Animate the sliding of the indicator in the Y axis */
    private IEnumerator slideIndicator(float targetY)
    {
        Vector2 anchoredPos = indicator.anchoredPosition;
        while (Mathf.Abs(targetY - anchoredPos.y) > 0.005f)
        {
            anchoredPos.y = Mathf.Lerp(anchoredPos.y, targetY, Time.deltaTime * 12.0f);
            indicator.anchoredPosition = anchoredPos;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        indicator.anchoredPosition = anchoredPos;
    }

    /* Hide all sub-menus on enable */
    public override void onShow()
    {
        for (int i = 0; i < subMenuHolder.childCount; ++i)
            StateController.Hide(subMenuHolder.GetChild(i).name);
        base.onShow();
    }
}
