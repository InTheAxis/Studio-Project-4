using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudDynamicsScript : MonoBehaviour
{
    public CloudScript cloudScript;
    public Vector3 moveRateA;
    public Vector3 moveRateB;

    private void Update()
    {
        cloudScript.offsetA += moveRateA * Time.deltaTime;
        cloudScript.offsetB += moveRateB * Time.deltaTime;
    }
}