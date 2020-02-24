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
                Play("footstep_stone0");
                break;

            case TerrainManager.TerrainType.DIRT:
                Play("footstep_dirt0");
                break;

            default:
                Debug.LogError("Terrain footstep audio not set.");
                break;
        }
    }

    public void PickUpDebris()
    {
        Play("pickup0");
    }

    public void Interact()
    {
        Play("interact0");
    }

    public void SabotageTower()
    {
        Play("sabotage0");
    }

    public void LaunchDebris()
    {
        Play("launch0");
    }
}