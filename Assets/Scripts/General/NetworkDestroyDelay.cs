using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkDestroyDelay : MonoBehaviour
{
    public float delay = 0.0f;
    private bool destroyed = false;

    private void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0.0f && !destroyed)
        {
            Debug.LogError(PhotonView.Get(this).ViewID);
            NetworkOwnership.instance.destroy(PhotonView.Get(this));
            destroyed = true;
        }
    }
}
