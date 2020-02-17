using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharTPCamera : MonoBehaviour
{
    public CharTPController target;
    
    [SerializeField]
    private float targetCamDist;

    private void FixedUpdate()
    {
        //move cam
        transform.position = target.transform.position - (target.lookDir * targetCamDist);
        //rotate cam
        transform.LookAt(target.transform);
    }
}
