using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MonsterHighlightNearby : MonoBehaviourPun
{
    [SerializeField]
    [Tooltip("Material used for highlighting nearby players when they are behind objects ")]
    private Material highlightBehind = null;

    [SerializeField]
    [Tooltip("The layer on which all objects are highlighted")]
    private string highlightLayer = "HighlightBehind";

    [SerializeField]
    [Tooltip("The layer for human players")]
    private string humanLayer = "Human";

    [SerializeField]
    [Tooltip("The range used for highlighting")]
    private float range = 15.0f;

    private int highlightLayerIndex = -1;
    private int playerLayerIndex = -1;

    private void Start()
    {
        highlightLayerIndex = LayerMask.NameToLayer(highlightLayer);
        if (highlightLayerIndex == -1)
            Debug.LogError(highlightLayer + " layer was not found!");
        playerLayerIndex = LayerMask.NameToLayer(humanLayer);
        if (playerLayerIndex == -1)
            Debug.LogError(humanLayer + " layer was not found!");
    }

    private void Update()
    {
        /* Run this only on monster's side aka MasterClient */
        if (!photonView.IsMine || !PhotonNetwork.IsMasterClient)
        {
            Destroy(this);
            return;
        }

        CharTPController[] players = FindObjectsOfType<CharTPController>();
        if(players != null && players.Length > 0)
        {
            for(int i = 0; i < players.Length; ++i)
            {
                /* Ignore self */
                if (players[i].gameObject == gameObject) continue;

                if (players[i].gameObject.layer == highlightLayerIndex && Vector3.Distance(players[i].transform.position, transform.position) > range)
                {
                    Transform[] children = players[i].gameObject.GetComponentsInChildren<Transform>();

                    if (children?.Length > 0)
                    {
                        foreach (Transform t in children)
                            t.gameObject.layer = playerLayerIndex;
                    }
                }
                else if (players[i].gameObject.layer != highlightLayerIndex && Vector3.Distance(players[i].transform.position, transform.position) <= range)
                {
                    Transform[] children = players[i].gameObject.GetComponentsInChildren<Transform>();

                    if (children?.Length > 0)
                    {
                        foreach (Transform t in children)
                            t.gameObject.layer = highlightLayerIndex;
                    }
                }
            }
        }
    }
}
