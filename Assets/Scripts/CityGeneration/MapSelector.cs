using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MapSelector : MonoBehaviour
{
    public enum Maps
    {
        City,
        Forest,
        Factory,
        MAX,
        RANDOM,
        NONE,
    }

    [SerializeField]
    private GameObject[] mapGenerators;

    [SerializeField]
    private Maps map;

    private static Maps sMap = Maps.NONE;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            gameObject.SetActive(false);
            return;
        }

        if (sMap != Maps.NONE)
            map = sMap;
        if (map == Maps.MAX)
        {
            Debug.LogError("MAP TYPE set to MAX. MAX is not a map type. Please select another");
            return;
        }
        if (map == Maps.RANDOM)
        {
            map = (Maps)(Random.Range(0, (int)Maps.MAX - 1));
        }
        LoadMap();
    }

    private void LoadMap()
    {
        InstantiateHandler.mInstantiate(mapGenerators[(int)map]);
    }

    public static void SelectMap(Maps map)
    {
        sMap = map;
    }
}