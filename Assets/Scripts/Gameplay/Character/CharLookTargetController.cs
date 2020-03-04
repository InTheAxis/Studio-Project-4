using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to control where camera should look
public class CharLookTargetController : MonoBehaviour
{
    public CharTPCamera tpCam;

    private struct InputData
    {
        public bool map;
        public bool front;
    };

    private InputData inp;
    private Minimap3D minimap = null;

    public event Action<bool> showMap;

    private void Update()
    {
        if (GameManager.playerObj == null)
            return;
        if (!GameManager.playerObj.GetComponent<CharTPController>().photonView.IsMine && Photon.Pun.PhotonNetwork.IsConnected)
            return;

        if (minimap == null)
        {
            minimap = GameManager.playerObj.GetComponentInChildren<Minimap3D>();
            minimap.gameObject.SetActive(false);
        }

        inp.map = Input.GetKeyDown(KeyCode.M);
        inp.front = Input.GetKeyDown(KeyCode.Alpha5);

        if (inp.map)
        {
            if (tpCam.IsLookingAtIdx() == 0 && !minimap.gameObject.activeSelf) //0 is always player
            {
                tpCam.LookAt("Map", 2);
                showMap?.Invoke(true);
                StartCoroutine(DelayOpenMap(0.3f));
            }
            else if(minimap.gameObject.activeSelf)
            {

                minimap.Hide();
                showMap?.Invoke(false);
                tpCam.LookAtPlayer();

            }
        }
        else if (inp.front)
        {
            if (tpCam.IsLookingAtIdx() == 0)
                tpCam.LookAt("Front", 4);
            else if (tpCam.IsLookingAt() == "Front")
                tpCam.LookAt("FrontSide", 4);
            else if (tpCam.IsLookingAt() == "FrontSide")
                tpCam.LookAtPlayer();
        }
    }

    private IEnumerator DelayOpenMap(float dura)
    {
        yield return new WaitForSeconds(dura);
        minimap.gameObject.SetActive(true);
    }
}