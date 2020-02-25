using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class Loading : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI tmDesc = null;
    [SerializeField]
    private TextMeshProUGUI tmTips = null;
    [SerializeField]
    private GameObject indicator = null;
    [SerializeField]
    private string[] descriptions = null;
    [SerializeField]
    private string[] tips = null;
    [SerializeField]
    private float pseudoLoadDuration = 2.0f;
    [SerializeField]
    private bool usePseudoLoading = false;

    private float loadTimer = 0.0f;

    public void Load(string targetScene)
    {
        indicator.SetActive(true);
        if (tmTips.text == "null")
            tmTips.text = tips[Random.Range(0, tips.Length)];

        if(usePseudoLoading)
            StartCoroutine(pseudoLoadScene(targetScene));
        else
            StartCoroutine(loadScene(targetScene));
    }

    public void LoadPhoton(string targetScene)
    {
        indicator.SetActive(true);
        if (tmTips.text == "null")
            tmTips.text = tips[Random.Range(0, tips.Length)];

        if (usePseudoLoading)
            StartCoroutine(pseudoLoadScenePhoton(targetScene));
        else
            StartCoroutine(loadScenePhoton(targetScene));
    }


    private IEnumerator loadScene(string targetScene)
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetScene);
        asyncOperation.allowSceneActivation = false;

        while(!asyncOperation.isDone)
        {

            /* Status */
            int descIndex = Mathf.Clamp((int)(asyncOperation.progress * descriptions.Length), 0, descriptions.Length - 1);
            int progress = Mathf.Clamp((int)(asyncOperation.progress * 100.0f), 0, 100);
            tmDesc.text = descriptions[descIndex] + ".." + progress.ToString() + "%";

            if(asyncOperation.progress >= 0.9f)
            {
                tmDesc.text = "Press Space to Continue";
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    asyncOperation.allowSceneActivation = true;
                    indicator.SetActive(false);
                }
            }
            yield return null;

        }
    }


    private IEnumerator pseudoLoadScene(string targetScene)
    {
        yield return null;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetScene);
        asyncOperation.allowSceneActivation = false;
        loadTimer = 0.0f;
        while (loadTimer < pseudoLoadDuration)
        {
            float ratio = loadTimer / pseudoLoadDuration;

            /* Status */
            int descIndex = Mathf.Clamp((int)(ratio * descriptions.Length), 0, descriptions.Length - 1);
            int progress = Mathf.Clamp((int)(ratio * 100.0f), 0, 100);
            tmDesc.text = descriptions[descIndex] + "..." + progress.ToString() + "%";

            loadTimer += Time.deltaTime;
            yield return null;

        }

        while (loadTimer >= pseudoLoadDuration)
        {
            tmDesc.text = "Press Space to Continue";
            if (Input.GetKeyDown(KeyCode.Space))
            {
                asyncOperation.allowSceneActivation = true;
                indicator.SetActive(false);
                loadTimer = 0.0f;
            }
            yield return null;
        }
    }


    private IEnumerator loadScenePhoton(string targetScene)
    {
        PhotonNetwork.LoadLevel(targetScene);
        yield return null;

        while (PhotonNetwork.LevelLoadingProgress < 1.0f)
        {

            /* Status */
            int descIndex = Mathf.Clamp((int)(PhotonNetwork.LevelLoadingProgress * descriptions.Length), 0, descriptions.Length - 1);
            int progress = Mathf.Clamp((int)(PhotonNetwork.LevelLoadingProgress * 100.0f), 0, 100);
            tmDesc.text = descriptions[descIndex] + ".." + progress.ToString() + "%";

            if (PhotonNetwork.LevelLoadingProgress >= 0.9f)
            {
                indicator.SetActive(false);
                //tmDesc.text = "Press Space to Continue";
                //if(Input.GetKeyDown(KeyCode.Space))
                //{
                //    asyncOperation.allowSceneActivation = true;
                //    indicator.SetActive(false);
                //}
            }
            yield return new WaitForEndOfFrame();

        }
    }

    private IEnumerator pseudoLoadScenePhoton(string targetScene)
    {
        loadTimer = 0.0f;
        while (loadTimer < pseudoLoadDuration)
        {
            float ratio = loadTimer / pseudoLoadDuration;

            /* Status */
            int descIndex = Mathf.Clamp((int)(ratio * descriptions.Length), 0, descriptions.Length - 1);
            int progress = Mathf.Clamp((int)(ratio * 100.0f), 0, 100);
            tmDesc.text = descriptions[descIndex] + "..." + progress.ToString() + "%";

            loadTimer += Time.deltaTime;
            yield return null;

        }

        PhotonNetwork.LoadLevel(targetScene);
        yield return null;
    }
}
