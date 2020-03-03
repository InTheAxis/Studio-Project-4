using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAudioController : AudioController
{
    [SerializeField]
    private LayerMask layerMask;

    private void Start()
    {
        SetSFX();
    }

    public void Step()
    {
        source = GetComponents<AudioSource>();
        // check terrain or misc
        RaycastHit hit;
        if (!Physics.Raycast(transform.position + new Vector3(0, 2, 0), -Vector3.up, out hit, 3, layerMask))
            return;

        if (hit.transform.tag == "Terrain")
        {
            TerrainManager.TerrainType type = TerrainManager.instance.GetActiveTerrainTextureIdx(transform.position);
            switch (type)
            {
                case TerrainManager.TerrainType.STONE:
                    PlayIndex(0);
                    break;

                case TerrainManager.TerrainType.DIRT:
                    PlayIndex(1);
                    break;

                case TerrainManager.TerrainType.Gravel:
                    PlayIndex(1);
                    break;

                default:
                    PlayIndex(0);
                    Debug.LogError("Terrain footstep audio not set.");
                    break;
            }
        }
        else
            PlayIndex(0);
    }
}