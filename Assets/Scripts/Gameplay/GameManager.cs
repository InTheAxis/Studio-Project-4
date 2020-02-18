using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private CharTPCamera playerCamera;

    public static GameObject playerObj = null;

    private void Awake()
    {
        playerObj = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0.0f, 4.0f, 0.0f), Quaternion.identity);
        playerCamera.target = playerObj.GetComponent<CharTPController>();
    }
}
