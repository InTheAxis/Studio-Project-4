using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviourPun
{
    public static GameManager instance = null;

    [SerializeField]
    private bool isOverride = false;

    [SerializeField]
    private bool isHuman = true;

    [SerializeField]
    private List<GameObject> modelPrefabs;

    public static GameObject playerObj = null;
    public static GameObject monsterObj = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Gamemanager instantiated twice! This should not happen");
    }

    private void Start()
    {
        photonView.RPC("remoteRequestSpawn", RpcTarget.MasterClient);

        StartCoroutine(retrieveMonsterRef());
    }

    [PunRPC]
    private void remoteRequestSpawn(PhotonMessageInfo messageInfo)
    {
        CityGenerator.instance.playerRequestedSpawn(messageInfo.Sender);
    }

    [PunRPC]
    private void Spawn(Vector3 pos)
    {
        // Preset as gameover so if we switch back to main menu before we receive game end RPC, we're set correctly
        StateGameover.isGameover = true;

        pos.y = 5; //< spawn in the air
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

        setCamera(playerObj);
    }

    public void sendForceGameEnd(bool isHunterWin)
    {
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("receivedForceGameEndRpc", RpcTarget.Others, isHunterWin);
        else
            Debug.LogError("sendForceGameEnd called on non-master client! This should not happen");
    }
    [PunRPC]
    private void receivedForceGameEndRpc(bool isHunterWin)
    {
        WinLose.instance?.receiveGameEnd(isHunterWin);
    }

    private IEnumerator retrieveMonsterRef()
    {
        MonsterEnergy comp = null;
        while ((comp = FindObjectOfType<MonsterEnergy>()) == null)
            yield return new WaitForSeconds(0.2f);

        monsterObj = comp.gameObject;
    }

    public static void setCamera(GameObject playerObj)
    {
        CharTPCamera.Instance?.SetCharController(playerObj.GetComponent<CharTPController>());

        //deprecated, shldn't need anymore, keeping vals here in case merge goes wrong
        /* Monster */
        //if (PhotonNetwork.IsMasterClient) //override camera values here
        //{
        //    CharTPCamera.Instance.SetTargetDist(2.4f); //if monster, set to further away
        //    CharTPCamera.Instance.SetCameraBob(0.005f, 4.0f);
        //}
        //else
        //{
        //    CharTPCamera.Instance.SetTargetDist(1.4f);
        //    CharTPCamera.Instance.SetCameraBob(0.02f, 9.0f);
        //}
    }

    public static void setCamera(CharTPController playerController)
    {
        Debug.Log(PlayerSettings.playerName + " => " + playerController.transform.name);
        CharTPCamera.Instance?.SetCharController(playerController);
    }
}