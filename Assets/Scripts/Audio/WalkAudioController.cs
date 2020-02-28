using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAudioController : AudioController
{
    [SerializeField]
    private LayerMask layerMask;

    public void Step()
    {
        source = GetComponents<AudioSource>();
        // check terrain or misc
        RaycastHit hit;
        if (!Physics.Raycast(transform.position + new Vector3(0, 1, 0), -Vector3.up, out hit, 2, layerMask))
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

                case TerrainManager.TerrainType.Gravel:
                    Play("footstep_dirt0");
                    break;

                default:
                    Play("footstep_stone0");
                    Debug.LogError("Terrain footstep audio not set.");
                    break;
            }
        }
        else
            Play("footstep_stone0");
    }
}