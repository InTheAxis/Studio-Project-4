using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunPreloadScene
{

	

#if UNITY_EDITOR

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void InitLoadingScene()
	{
        //if (SceneManager.GetSceneByName("Preload") != null)
        //    SceneManager.LoadScene("Preload");

    }
#endif
}
