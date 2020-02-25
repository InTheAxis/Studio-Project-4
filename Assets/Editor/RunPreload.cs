using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RunPreloadScene
{



#if UNITY_EDITOR

    private const string MENU_ITEM = "Addons/Run Preload";

    private static bool toggled = EditorPrefs.GetBool(MENU_ITEM);

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void InitLoadingScene()
	{
        if (toggled && SceneManager.GetSceneByName("Mainmenu") != null)
            SceneManager.LoadScene("Mainmenu");

    }

    [MenuItem(MENU_ITEM)]
    public static void Toggle()
    {
        toggled = !toggled;
        Menu.SetChecked(MENU_ITEM, toggled);
        EditorPrefs.SetBool(MENU_ITEM, toggled);
    }
#endif
}
