using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTutorial : State
{
    [Header("References")]
    [SerializeField]
    [Tooltip("The holder that contains all sub-menus")]
    private RectTransform subMenuHolder = null;
    [SerializeField]
    [Tooltip("The sliding indicator that shows what sub-menu is opened at the moment.")]
    private RectTransform indicator = null;


    public override string Name { get { return "Tutorial"; } }
    private IEnumerator iSlideIndicator = null;
    private GameObject currentSubMenu = null;

    private void Start()
    {
        
    }

    private void Update()
    {
        
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

    public void selectSubMenu(GameObject go)
    {
        selectNavButton(go);
        //StateController.Show(go.name);
        GameObject subMenu = subMenuHolder.Find(go.name).gameObject;
        if(subMenu != null)
        {
            currentSubMenu.SetActive(false);
            currentSubMenu = subMenu;
            currentSubMenu.SetActive(true);
        }
    }

    public void backToLobby()
    {
        StateController.Hide(Name);
    }

    public override void onShow()
    {
        /* Reset sub menu states */
        currentSubMenu = subMenuHolder.GetChild(0).gameObject;
        currentSubMenu.SetActive(true);
        for (int i = 1; i < subMenuHolder.childCount; ++i)
            subMenuHolder.GetChild(i).gameObject.SetActive(false);

        base.onShow();
        StartCoroutine(StateController.fadeCanvasGroup(GetComponent<CanvasGroup>(), true, 10.0f));
    }
}
