using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudQuad : MonoBehaviour
{
    private void Update()
    {
        Vector3[] frustumCorners = new Vector3[4];
        Camera.main.CalculateFrustumCorners(new Rect(0, 0, 1, 1), transform.localPosition.z, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);
        Vector3 scale = new Vector3();
        var worldSpaceCornerA = Camera.main.transform.TransformVector(frustumCorners[0]);
        var worldSpaceCornerB = Camera.main.transform.TransformVector(frustumCorners[0]);
        var worldSpaceCornerC = Camera.main.transform.TransformVector(frustumCorners[2]);
        scale = frustumCorners[2] - frustumCorners[0];
        transform.localScale = scale;
    }
}