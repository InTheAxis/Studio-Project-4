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
    public float camAdjustSpeed = 5;
    public float camOccludeSpeed = 50;
    public float adjustOffset = 0.3f;

    private Transform target;
    private Transform nextTarget;
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
        //create and attach a tracking point
        GameObject go = new GameObject("CameraTarget (Dynamic)");
        go.transform.parent = cc.lookTargets[0].parent;
        //init target
        target = go.transform;
        target.position = lookTargets[0].position;
        nextTarget = lookTargets[0];
    }

    //use me to look at the transform at index of the target array
    public void LookAt(int index, float distToTarget)
    {
        if (lookTargets.Count < 1)
            return;
        nextTarget = lookTargets[index];
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

        if (charControl == null)
        {
            Debug.LogWarning("Character controller not found");
            target = new GameObject("CameraTarget (Dynamic)").transform;
            target.position = Vector3.zero;
            nextTarget = target;
        }

        cam = GetComponent<Camera>();

        clipPoint = new Vector3[5];
    }   
    private void LateUpdate()
    {
        //TEMP ,DO THIS ELSEWHERE
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (IsLookingAt() == 0)
                LookAt(1, 2);
            else
                LookAtPlayer();
        }

        if ((target.position - nextTarget.position).magnitude > 0)
        { 
            target.position = Vector3.Slerp(target.position, nextTarget.position, Time.deltaTime * camAdjustSpeed);
           target.rotation = Quaternion.Slerp(target.rotation, nextTarget.rotation, Time.deltaTime * camAdjustSpeed);
        }

        //calculate if shld move closer
        UpdateCameraClipPoints();
        CalculateObstruction();
        curCamDist = Mathf.Lerp(curCamDist, moveCloserDist, Time.deltaTime * (moveCloserDist == targetCamDist ? camAdjustSpeed : camOccludeSpeed));

        //if (moveCloserDist < 2)
        //    Debug.Log("player shld become transluscent");

        //move cam
        desiredPos = target.transform.position - (charControl.lookDir * targetCamDist);
        transform.position = target.transform.position - (charControl.lookDir * curCamDist);
        //rotate cam
        transform.rotation = Quaternion.LookRotation(charControl.lookDir, Vector3.up);//+ new Vector3(target.rotation.x, 0, target.rotation.z);
    }

    private void UpdateCameraClipPoints()
    {
        float z = cam.nearClipPlane + adjustOffset;
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
        float minDist = targetCamDist;
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
        moveCloserDist = Mathf.Clamp(minDist, 0.1f, targetCamDist);
    }
}
