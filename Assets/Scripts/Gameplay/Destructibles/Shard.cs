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

        //rigidbodies = GetComponentsInChildren<Rigidbody>();
        //foreach (Rigidbody rb in rigidbodies)
        //{
            if (Random.Range(0.0f, 1.0f) <= chanceToDespawn)
            {
                Debug.Log("Destroyed");

                //Destroy(rb.gameObject, timeBeforeDespawn);
                if (GetComponent<NetworkDestroyDelay>() == null)
                {
                    NetworkDestroyDelay comp = this.gameObject.AddComponent<NetworkDestroyDelay>();
                    comp.delay = timeBeforeDespawn;
                }
                //continue;
            }

            //Vector3 size = rb.GetComponent<MeshCollider>().bounds.size;

            //float scale = size.x * transform.localScale.x;
            //scale = Mathf.Max(scale, size.y * transform.localScale.y);
            //scale = Mathf.Max(scale, size.z * transform.localScale.z);

            //if(scale <= smallShardScaleThreshold)
            //    smallShards.Add(rb);
        //}
    }

    private void Update()
    {
        //if (!NetworkOwnership.objectIsOwned(thisView))
        //    return;

        //foreach (Rigidbody rb in smallShards)
        //{
        //    if(rb.velocity.sqrMagnitude <= shardSettleVelocityThreshold)
        //        sleepShards.Add(rb);
        //}

        //foreach(Rigidbody rb in sleepShards)
        //{
        //    smallShards.Remove(rb);
        //    GameObject go = rb.gameObject;
        //    Destroy(rb);
        //    Destroy(go.GetComponent<MeshCollider>());
        //    Debug.Log("Useless");
        //}
        //sleepShards.Clear();
    }
}
