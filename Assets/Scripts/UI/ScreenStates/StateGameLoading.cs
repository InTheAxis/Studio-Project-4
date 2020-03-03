using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateGameLoading : State
{

    [SerializeField]
    private bool usePseudoLoading = false;

    public override string Name { get { return "GameLoading"; } }


    private void Start()
    {
        
    }

    private void Update()
    {
        
    }


    public void loadPhoton(string targetScene)
    {

        //if (tmTips.text == "null")
        //    tmTips.text = tips[Random.Range(0, tips.Length)];

        if (usePseudoLoading)
            StartCoroutine(pseudoLoadScenePhoton(targetScene));
        else
            StartCoroutine(loadScenePhoton(targetScene));
    }

    private IEnumerator loadScenePhoton(string targetScene)
    {
        PhotonNetwork.LoadLevel(targetScene);
        yield return null;

        while (PhotonNetwork.LevelLoadingProgress < 1.0f)
        {

            /* Status */
            //int descIndex = Mathf.Clamp((int)(PhotonNetwork.LevelLoadingProgress * descriptions.Length), 0, descriptions.Length - 1);
            //int progress = Mathf.Clamp((int)(PhotonNetwork.LevelLoadingProgress * 100.0f), 0, 100);
            //tmDesc.text = descriptions[descIndex] + ".." + progress.ToString() + "%";

            if (PhotonNetwork.LevelLoadingProgress >= 0.9f)
            {

                //indicator.SetActive(false);
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
        //loadTimer = 0.0f;
        //while (loadTimer < pseudoLoadDuration)
        //{
        //    float ratio = loadTimer / pseudoLoadDuration;

        //    /* Status */
        //    int descIndex = Mathf.Clamp((int)(ratio * descriptions.Length), 0, descriptions.Length - 1);
        //    int progress = Mathf.Clamp((int)(ratio * 100.0f), 0, 100);
        //    tmDesc.text = descriptions[descIndex] + "..." + progress.ToString() + "%";

        //    loadTimer += Time.deltaTime;
        //    yield return null;

        //}

        //PhotonNetwork.LoadLevel(targetScene);
        yield return null;
    }

}
