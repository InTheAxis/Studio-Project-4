using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool isOverride = false;
    [SerializeField]
    private bool isHuman = true;
    [SerializeField]
    private GameObject humanPrefab;
    [SerializeField]
    private GameObject monsterPrefab;

    public static GameObject playerObj = null;


    private void Awake()
    {
        if(!isOverride)
            isHuman = !PhotonNetwork.IsMasterClient;

        playerObj = PhotonNetwork.Instantiate(isHuman ? humanPrefab.name : monsterPrefab.name, new Vector3(0.0f, 4.0f, 0.0f), Quaternion.identity);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        setCamera(playerObj);
    }

    private void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;        
    }

    public static void setCamera(GameObject playerObj)
    {
        CharTPCamera.Instance?.SetCharController(playerObj.GetComponent<CharTPController>());
    }
    public static void setCamera(CharTPController playerController)
    {
        CharTPCamera.Instance?.SetCharController(playerController);
    }
}
