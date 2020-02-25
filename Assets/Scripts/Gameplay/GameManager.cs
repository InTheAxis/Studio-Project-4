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
        if (CharTPCamera.Instance != null)
            CharTPCamera.Instance.SetCharController(playerObj.GetComponent<CharTPController>());    
    }

    private void Start()
    {
        if (CharTPCamera.Instance != null)
            CharTPCamera.Instance.SetCharController(playerObj.GetComponent<CharTPController>());
    }
}
