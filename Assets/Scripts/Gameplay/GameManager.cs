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
        int prefabIndex = (int)NetworkClient.getPlayerProperty("charModel");
        Debug.Log("Got prefab index " + prefabIndex);

        if (isOverride)
            prefabIndex = isHuman ? 1 : 0;

        Debug.Log("Instantiating prefab of index " + prefabIndex);
        if (prefabIndex == 0)
            playerObj = PhotonNetwork.Instantiate(monsterPrefab.name, new Vector3(0.0f, 4.0f, 0.0f), Quaternion.identity);
        else
        {
            playerObj = PhotonNetwork.Instantiate(humanPrefab.name, new Vector3(0.0f, 4.0f, 0.0f), Quaternion.identity);
            playerObj.GetComponent<CharModelSelect>().SelectModel(prefabIndex - 1);
        }
    }

    private void Start()
    {
        setCamera(playerObj);
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
