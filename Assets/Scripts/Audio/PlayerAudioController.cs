using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : AudioController
{
    public void Step()
    {
        TerrainManager.TerrainType type = TerrainManager.instance.GetActiveTerrainTextureIdx(transform.position);
        switch (type)
        {
            case TerrainManager.TerrainType.STONE:
                Play("footstep_stone");
                break;

            case TerrainManager.TerrainType.DIRT:
                Play("footstep_dirt");
                break;

            default:
                Debug.LogError("Terrain footstep audio not set.");
                break;
        }
    }
}