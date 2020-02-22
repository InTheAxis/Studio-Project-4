using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Shard : MonoBehaviour
{
    private Rigidbody[] rigidbodies = null;

    [SerializeField]
    private float chanceToDespawn = 0.30f;
    [SerializeField]
    private float timeBeforeDespawn = 20.0f;
    [SerializeField]
    private float smallShardScaleThreshold = 1.0f;
    [SerializeField]
    private float shardSettleVelocityThreshold = 0.1f;
    private bool shouldDespawn = false;


    private List<Rigidbody> smallShards = new List<Rigidbody>();
    private List<Rigidbody> sleepShards = new List<Rigidbody>();

    private PhotonView thisView;

    private void Start()
    {
        thisView = PhotonView.Get(this);
        if (!PhotonNetwork.IsMasterClient)
            return;


        if (Random.Range(0.0f, 1.0f) <= chanceToDespawn)
        {
            //Debug.Log("Destroyed");

            if (GetComponent<NetworkDestroyDelay>() == null)
            {
                NetworkDestroyDelay comp = this.gameObject.AddComponent<NetworkDestroyDelay>();
                comp.delay = timeBeforeDespawn;
            }

        }


    }

}
