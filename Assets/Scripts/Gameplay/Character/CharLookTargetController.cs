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
        if (minimap == null && GameManager.playerObj != null)
        {
            minimap = GameManager.playerObj.GetComponentInChildren<Minimap3D>();
            minimap.gameObject.SetActive(false);
        }

        inp.map = Input.GetKeyDown(KeyCode.M);
        inp.front = Input.GetKeyDown(KeyCode.O);

        if (inp.map)
        {
            if (tpCam.IsLookingAtIdx() == 0) //0 is always player
            {
                tpCam.LookAt("Map", 2);

                minimap.gameObject.SetActive(true);
                showMap?.Invoke(true);
            }
            else
            {
                minimap.Hide();
                showMap?.Invoke(false);
                tpCam.LookAtPlayer();
            }
        }
        else if (inp.front)
        {
            if (tpCam.IsLookingAtIdx() == 0)
                tpCam.LookAt("Front", 2);
            else if (tpCam.IsLookingAt() == "Front")
                tpCam.LookAt("FrontSide", 2);
            else if (tpCam.IsLookingAt() == "FrontSide")
                tpCam.LookAtPlayer();
        }
    }
}