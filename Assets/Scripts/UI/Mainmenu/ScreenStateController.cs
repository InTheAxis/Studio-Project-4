using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenStateController : MonoBehaviour
{

    private enum ScreenStates
    {
        LOGIN,
        MAINMENU,
        SERVERSELECT,
        MATCHLOBBY,
        OPTIONS,
        CREDITS,
        COUNT,
    }

    [Header("Screens")]
    [SerializeField]
    [Tooltip("Stores a collection of different menu screens")]
    private GameObject[] screens = null;

    [Header("Interactions")]
    [SerializeField]
    [Tooltip("The size of the button when hovered")]
    private float hoverGrowSize = 1.2f;

    [Header("Transitions")]
    [SerializeField]
    private ButtonMaskEffect buttonMask = null;
    [SerializeField]
    private float buttonMaskStartY = 78.0f;
    [SerializeField]
    private float buttonHeightGap = 120.0f;

    [Header("Server Selection")]
    [SerializeField]
    private GameObject hostPasswordInput = null;
    [SerializeField]
    private GameObject joinPasswordInput = null;

    private List<GameObject> hovered;
    private List<GameObject> prevHovered;
    private List<RaycastResult> raycastResults;

    /* Interactions */
    private Vector3 hoverGrowScale = Vector3.zero;

    /* Screen State */
    [SerializeField]
    private ScreenStates currentScreen = ScreenStates.MAINMENU;

    private Stack<ScreenStates> history = null;



    private void Start()
    {
        history = new Stack<ScreenStates>();
        hovered = new List<GameObject>();
        prevHovered = new List<GameObject>();
        hoverGrowScale = new Vector3(hoverGrowSize, hoverGrowSize, hoverGrowSize);

        for(int i = 0; i < screens.Length; ++i)
        {
            if (i == (int)currentScreen)
                screens[i].SetActive(true);
            else
                screens[i].SetActive(false);
        }
    }

    private void Update()
    {

    }


    private void getHoveredUIElements()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
    }


    public void onButtonClick(string name)
    {
        if(name == "Back")
        {
            screens[(int)currentScreen].SetActive(false);
            currentScreen = history.Pop();
            screens[(int)currentScreen].SetActive(true);
        }
        else if(name == "MainmenuPlay")
        {
            screens[(int)ScreenStates.SERVERSELECT].SetActive(true);
            screens[(int)ScreenStates.MAINMENU].SetActive(false);
            history.Push(currentScreen);
            currentScreen = ScreenStates.SERVERSELECT;
            buttonMask.Begin(buttonMaskStartY + buttonHeightGap * 3);

            joinPasswordInput.SetActive(false);
            hostPasswordInput.SetActive(false);
        }
        else if(name == "MainmenuOptions")
        {
            buttonMask.Begin(buttonMaskStartY + buttonHeightGap * 2);
        }
        else if(name == "MainmenuCredits")
        {
            buttonMask.Begin(buttonMaskStartY + buttonHeightGap * 1);
        }
        else if (name == "Exit")
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
        else if(name == "ServerSelectHost")
        {
            hostPasswordInput.SetActive(true);
            joinPasswordInput.SetActive(false);

        }
        else if(name == "ServerJoin")
        {
            joinPasswordInput.SetActive(true);
            hostPasswordInput.SetActive(false);

        }
        else if(name == "ServerJoinRandom")
        {

        }
    }

    public void onHoverEnterButton(GameObject go)
    {
        go.transform.localScale = hoverGrowScale;
    }

    public void onHoverExitButton(GameObject go)
    {
        go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
