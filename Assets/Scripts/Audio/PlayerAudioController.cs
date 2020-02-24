using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : AudioController
{
    [SerializeField]
    private LayerMask layerMask;

    public void Step()
    {
        // check terrain or misc
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, -Vector3.up, out hit, 2, layerMask))
            return;

        if (hit.transform.tag == "Terrain")
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
        else
            Play("footstep_stone0");
    }

    public void PickUpDebris()
    {
        Play("pickup0", 0.22f);
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
        Play("launch0", 0.3f);
    }
}