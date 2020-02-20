using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

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

    [Header("Login")]
    [SerializeField]
    private TMP_InputField tmLoginUsername = null;
    [SerializeField]
    private TMP_InputField tmLoginPassword = null;
    [SerializeField]
    private TextMeshProUGUI tmLoginStatus = null;

    [Header("Registration")]
    [SerializeField]
    private TMP_InputField tmRegisterEmail = null;
    [SerializeField]
    private TMP_InputField tmRegisterUsername = null;
    [SerializeField]
    private TMP_InputField tmRegisterPassword = null;
    [SerializeField]
    private TMP_InputField tmRegisterConfirm = null;
    [SerializeField]
    private TextMeshProUGUI tmRegisterStatus = null;

    [Header("Server Selection")]
    [SerializeField]
    private GameObject registerInput = null;
    [SerializeField]
    private GameObject loginInput = null;
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

    private void setScene(ScreenStates targetState)
    {
        screens[(int)targetState].SetActive(true);
        screens[(int)currentScreen].SetActive(false);
        history.Push(currentScreen);
        currentScreen = targetState;
    }


    public void onButtonClick(string name)
    {
        if (name == "Back")
        {
            screens[(int)currentScreen].SetActive(false);
            currentScreen = history.Pop();
            screens[(int)currentScreen].SetActive(true);
            buttonMask.Begin(buttonMaskStartY);
        }
        else if (name == "Login")
        {
            /* Integrate Playfab Login */
            tmLoginStatus.text = "Connecting...";

            setScene(ScreenStates.MAINMENU);
        }
        else if (name == "Register")
        {

            /* Password mismatch */
            if (!tmRegisterPassword.text.Equals(tmRegisterConfirm.text))
            {
                tmRegisterStatus.text = "Password Mismatch";
            }
            else
            {
                tmRegisterStatus.text = "Registering";
                /* Integrate Playfab Register */

            }

        }
        else if (name == "Logout")
        {
            /* Integrate Playfab Logout */

            setScene(ScreenStates.LOGIN);
            buttonMask.Begin(buttonMaskStartY);
            loginInput.SetActive(false);
            registerInput.SetActive(false);
        }
        else if (name == "MainmenuLogin")
        {
            loginInput.SetActive(!loginInput.activeSelf);
            registerInput.SetActive(false);
        }
        else if (name == "MainmenuRegister")
        {
            loginInput.SetActive(false);
            registerInput.SetActive(!registerInput.activeSelf);
        }
        else if (name == "MainmenuPlay")
        {
            setScene(ScreenStates.SERVERSELECT);
            buttonMask.Begin(buttonMaskStartY + buttonHeightGap * 3);

            joinPasswordInput.SetActive(false);
            hostPasswordInput.SetActive(false);
        }
        else if (name == "MainmenuOptions")
        {
            buttonMask.Begin(buttonMaskStartY + buttonHeightGap * 2);
        }
        else if (name == "MainmenuCredits")
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
        else if (name == "ServerSelectHost")
        {
            hostPasswordInput.SetActive(true);
            joinPasswordInput.SetActive(false);

        }
        else if (name == "ServerJoin")
        {
            joinPasswordInput.SetActive(true);
            hostPasswordInput.SetActive(false);

        }
        else if (name == "ServerJoinRandom")
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
