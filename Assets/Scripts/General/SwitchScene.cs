using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class SwitchScene : MonoBehaviour
{
    public void switchScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
