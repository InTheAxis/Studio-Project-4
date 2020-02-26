using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CityGenerator : MonoBehaviour
{
    public static CityGenerator instance = null;

    //public List<Generator> generators;
    public List<GeneratorData> generators;

    public float scale = 1;

    // public bool serverInstantiate = false;

    public bool generateOnStart = false;

    private bool generated = false;
    private List<Player> spawnRequestingPlayers = new List<Player>();

    private List<Vector3> freePlayerSpawnPos = new List<Vector3>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("CityGenerator instantiated twice! This should not happen");
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient && generateOnStart)
            Generate();
    }

    private void OnValidate()
    {
        foreach (GeneratorData generator in generators)
        {
            generator.generator.SetScale(scale);
        }
    }

    public void Clear()
    {
        foreach (GeneratorData generator in generators)
        {
            generator.generator.Clear();
        }
    }

    public void Generate()
    {
        Debug.Log("Generation began");
        Clear();

        foreach (GeneratorData generator in generators)
        {
            if (generator.enabled)
                generator.generator.Generate();
        }
        Debug.Log("Generation complete");
        // generation complete
        generated = true;

        foreach (Player player in spawnRequestingPlayers)
            sendSpawnDirective(player);
        spawnRequestingPlayers.Clear();
    }
    private void sendSpawnDirective(Player player)
    {
        Debug.Log("Sending spawn directive to " + player.ActorNumber);

        if (freePlayerSpawnPos.Count == 0)
            freePlayerSpawnPos = new List<Vector3>(PlayerSpawnGenerator.playerSpawnPos);

        if ((int)NetworkClient.getPlayerProperty(player, "charModel") == 0) // Hunter
            GameManager.instance.photonView.RPC("Spawn", player, PlayerSpawnGenerator.hunterSpawnPos);
        else // Survivor
        {
            int thisPlayerSpawnPosIndex = Random.Range(0, freePlayerSpawnPos.Count);
            GameManager.instance.photonView.RPC("Spawn", player, freePlayerSpawnPos[thisPlayerSpawnPosIndex]);
            freePlayerSpawnPos.RemoveAt(thisPlayerSpawnPosIndex);
        }
    }

    public void playerRequestedSpawn(Player player)
    {
        if (generated)
            sendSpawnDirective(player);
        else
            spawnRequestingPlayers.Add(player);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(Vector3.zero, scale);
    }
}