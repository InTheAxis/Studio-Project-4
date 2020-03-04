using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LayerMaskExt
{
    //check if layer is ignored, which means its checked in the mask
    public static bool CheckIfInMask(LayerMask mask, int layerToCheck)
    {
        //ignore bits AND (1 bit shifted to where layer bit is)
        //if 0 means none matched, which means ignored
        return (mask & (1 << layerToCheck)) != 0;
    }

    //check if layer is not ignored, which means its not checked in the mask
    public static bool CheckIfNotInMask(LayerMask mask, int layerToCheck)
    {
        //ignore bits AND (1 bit shifted to where layer bit is)
        //if 0 means none matched, which means ignored
        return (mask & (1 << layerToCheck)) == 0;
    }
};
