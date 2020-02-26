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
    private List<GameObject> modelPrefabs;

    public static GameObject playerObj = null;

    private void Awake()
    {
        Spawn(Vector3.zero);
    }

    public void Spawn(Vector3 pos)
    {
        int prefabIndex = (int)NetworkClient.getPlayerProperty("charModel");
        Debug.Log("Got prefab index " + prefabIndex);

        if (isOverride)
            prefabIndex = isHuman ? 1 : 0;

        if (prefabIndex >= modelPrefabs.Count || prefabIndex < 0)
            Debug.LogErrorFormat("Invalid idx of {0}", prefabIndex);
        else
        {
            Debug.Log("Instantiating prefab of index " + prefabIndex);
            playerObj = PhotonNetwork.Instantiate(modelPrefabs[prefabIndex].name, pos, Quaternion.identity);
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