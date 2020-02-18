using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharTPCamera : MonoBehaviour
{
    public List<Transform> lookTargets;
    public CharTPController charControl;    
    
    public float targetCamDist;
    public float moveCloserDuration;
    public float initialCamDist;
    public LayerMask mask;
    public float camAdjustSpeed = 10;
    public float adjustOffset = 1;

    private Transform target;
    private Camera cam;
    private int targetIdx;
    private float defaultCamDist;
    private float curCamDist;
    private float moveCloserDist;
    private Vector3[] clipPoint;
    private Vector3 desiredPos;

    #region Public Calls
    //attach char controller for that look direction
    public void GiveMeCharController(CharTPController cc)
    {
        charControl = cc;
        lookTargets = cc.lookTargets;
        target = lookTargets[0];
    }

    //use me to look at the transform at index of the target array
    public void LookAt(int index, float distToTarget)
    {
        target = lookTargets[index];
        targetCamDist = distToTarget;
        targetIdx = index;
    }

    //returns the index of transform in target array
    public int IsLookingAt() 
    {
        return targetIdx;
    }

    //use me to look at player (index 0 and default dist)
    public void LookAtPlayer()
    {
        LookAt(0, defaultCamDist);
    }
    #endregion

    private void Start()
    {
        defaultCamDist = targetCamDist;
        moveCloserDist = targetCamDist;
        curCamDist = initialCamDist;

        targetIdx = 0;
        target = lookTargets[targetIdx];

        cam = GetComponent<Camera>();

        clipPoint = new Vector3[5];
    }   
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (IsLookingAt() == 0)
                LookAt(1, 2);
            else
                LookAtPlayer();
        }

        //calculate if shld move closer
        UpdateCameraClipPoints();
        CalculateObstruction();
        curCamDist = Mathf.Lerp(curCamDist, moveCloserDist, Time.deltaTime * camAdjustSpeed);

        if (moveCloserDist < 2)
            Debug.Log("player shld become transluscent");

        //move cam
        desiredPos = target.transform.position - (charControl.lookDir * targetCamDist);
        transform.position = target.transform.position - (charControl.lookDir * curCamDist);
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
