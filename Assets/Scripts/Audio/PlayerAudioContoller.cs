using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioContoller : AudioController
{
    //[SerializeField]
    //private LayerMask layerMask;

    //public void Step()
    //{
    //    // check terrain or misc
    //    RaycastHit hit;
    //    if (!Physics.Raycast(transform.position, -Vector3.up, out hit, 2, layerMask))
    //        return;

    //    if (hit.transform.tag == "Terrain")
    //    {
    //        TerrainManager.TerrainType type = TerrainManager.instance.GetActiveTerrainTextureIdx(transform.position);
    //        switch (type)
    //        {
    //            case TerrainManager.TerrainType.STONE:
    //                Play("footstep_stone0");
    //                break;

    //            case TerrainManager.TerrainType.DIRT:
    //                Play("footstep_dirt0");
    //                break;

    //            default:
    //                Debug.LogError("Terrain footstep audio not set.");
    //                break;
    //        }
    //    }
    //    else
    //        Play("footstep_stone0");
    //}
    private void Start()
    {
        SetSFX();
    }

    public void PickUpDebris()
    {
        Play("pickup0");
        Play("held0", 1);
    }

    //public void Interact()
    //{
    //    Play("interact0");
    //}

    //public void SabotageTower()
    //{
    //    Play("sabotage0");
    //}

    public void DropDebris()
    {
        SetAudio("held0", AudioState.Stop, 1);
    }

    public void LaunchDebris()
    {
        Play("throw0");
        SetAudio("held0", AudioState.Stop, 1);
    }
}