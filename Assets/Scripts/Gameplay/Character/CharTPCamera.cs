using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharTPCamera : MonoBehaviour
{
    public CharTPController target;    
    public float targetCamDist;
    public float moveCloserDuration;
    public float initialCamDist;
    public LayerMask mask;
    public float camAdjustSpeed = 10;
    public float adjustOffset = 1;

    private float curCamDist;
    private float timer;
    private float moveCloserDist;
    private Camera cam;
    private Vector3[] clipPoint;
    private Vector3 desiredPos;

    private void Start()
    {
        curCamDist = initialCamDist;
        moveCloserDist = targetCamDist;
        timer = 0;

        cam = GetComponent<Camera>();

        clipPoint = new Vector3[5];
        UpdateCameraClipPoints();
    }   
    private void LateUpdate()
    {
        //calculate if shld move closer
        CalculateObstruction();
        curCamDist = Mathf.Lerp(curCamDist, moveCloserDist, Time.deltaTime * camAdjustSpeed);

        if (moveCloserDist < 2)
            Debug.Log("player shld become transluscent");

        //move cam
        desiredPos = target.transform.position - (target.lookDir * targetCamDist);
        transform.position = target.transform.position - (target.lookDir * curCamDist);
        //rotate cam
        transform.LookAt(target.transform);
    }

    private void UpdateCameraClipPoints()
    {
        float z = cam.nearClipPlane;
        float x = Mathf.Tan(Mathf.Deg2Rad * cam.fieldOfView * 0.5f) * z;
        float y = x / cam.aspect;
        Quaternion rot = Quaternion.LookRotation(transform.forward, transform.up);

        clipPoint[0] = rot * new Vector3(x, y, z) + desiredPos;
        clipPoint[1] = rot * new Vector3(-x, y, z) + desiredPos;
        clipPoint[2] = rot * new Vector3(x, -y, z) + desiredPos;
        clipPoint[3] = rot * new Vector3(-x, -y, z) + desiredPos;
        clipPoint[4] = desiredPos - transform.forward;

        Debug.DrawLine(clipPoint[0], target.transform.position, Color.white);
        Debug.DrawLine(clipPoint[1], target.transform.position, Color.white);
        Debug.DrawLine(clipPoint[2], target.transform.position, Color.white);
        Debug.DrawLine(clipPoint[3], target.transform.position, Color.white);
        Debug.DrawLine(clipPoint[4], target.transform.position, Color.green);
    }

    private void CalculateObstruction()
    {
        UpdateCameraClipPoints();
        float minDist = targetCamDist + adjustOffset;
        Vector3 dir;
        float dist;
        for (int i = 0; i < 5; ++i)
        {
            dir = clipPoint[i] - target.transform.position;
            if (Physics.Raycast(target.transform.position, dir.normalized, out RaycastHit hitInfo, dir.magnitude, mask))
            {
                dist = hitInfo.distance;
                if (dist < minDist)
                    minDist = dist;
            }
            
        }
        moveCloserDist = Mathf.Clamp(minDist - adjustOffset, 0.1f, targetCamDist);
    }
}
