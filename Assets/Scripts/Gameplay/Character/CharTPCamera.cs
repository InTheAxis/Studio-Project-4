using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//===================Make sure there is only one of this script in each scene!
public class CharTPCamera : MonoBehaviour
{
    public static CharTPCamera Instance;
    
    //[Header("References from Character Object")]
    //[SerializeField]
    //[Tooltip("Should be sett externaly with this class' function")]
    private CharTPController charControl;
    //[SerializeField]
    //[Tooltip("What the camera should look at and follow")]
    private List<Transform> lookTargets;

    [Header("Distance to Target")]
    [SerializeField] 
    [Tooltip("How far the camera should be away from look target")]
    private float targetCamDist;
    [SerializeField]
    [Tooltip("At what distance away should the camera start")]
    private float initialCamDist;
    [Header("Camera Collision Settings")]
    [SerializeField]
    [Tooltip("What layers will trigger the collision")]
    private LayerMask mask;
    [SerializeField]
    [Tooltip("How fast the camera will lerp to targets")]
    private float camAdjustSpeed = 5;
    [SerializeField]
    [Tooltip("How fast the camera will lerp when occluded")]
    private float camOccludeSpeed = 50;
    [SerializeField]
    [Tooltip("Offsets where the rays are cast from, higher means the camera detects occlusions earlier")]
    private float adjustOffset = 0.3f;

    [Header("Camera Shake Settings")]
    [SerializeField]
    [Tooltip("How long to shake cam")]
    private float shakeDuration = 0.5f;
    [SerializeField]
    [Tooltip("How far to shake cam")]
    private float shakeAmplitude = 0.01f;
    [SerializeField]
    [Tooltip("How frequent to shake cam")]
    private float shakeFrequency = 0.5f;

    [Header("Crosshair")]
    [SerializeField]
    [Tooltip("Crosshair Sprite")]
    private Transform crosshair = null;


    private Transform target;
    private Transform nextTarget;
    private Camera cam;
    private int targetIdx;
    private Vector3 rotatedLookDir;
    private float defaultCamDist;
    private float curCamDist;
    private float moveCloserDist;
    private Vector3[] clipPoint;
    private Vector3 desiredPos;
    private IEnumerator camShakeCorr;

    #region private Calls
    //attach char controller for that look direction
    public void SetCharController(CharTPController cc)
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
        var col = nextTarget.GetComponent<Collider>();
        if (col) col.enabled = false;
        col = lookTargets[index].GetComponent<Collider>();
        if (col) col.enabled = true;

        nextTarget = lookTargets[index];
        targetCamDist = distToTarget;
        targetIdx = index;
    }
    public void LookAt(string _name, float distToTarget)
    {
        int idx = -1;
        for (int i = 0; i < lookTargets.Count; ++i)
        {
            if (lookTargets[i].name == _name)
            {
                idx = i;
                break;
            } 
        }
        if (idx < 0)
        {
            Debug.LogErrorFormat("Can't find lookTarget {0}", _name);
            return;
        }

        LookAt(idx, distToTarget);
    }

    //returns the index of transform in target array
    public int IsLookingAtIdx() 
    {
        return targetIdx;
    }
    
    //returns name of target being looked at
    public string IsLookingAt()
    {
        return nextTarget.name;
    }

    //use me to look at player (index 0 and default dist)
    public void LookAtPlayer()
    {
        LookAt(0, defaultCamDist);
    }

    //use me to shake camera at deafult settings
    public void Shake()
    {
        Shake(shakeDuration, shakeAmplitude, shakeFrequency);
    }
    //use me to shake camera with overriden settings
    public void Shake(float duration, float amplitude, float frequency)
    {
        if (camShakeCorr != null)
            StopCoroutine(camShakeCorr);
        nextTarget = lookTargets[targetIdx];
        camShakeCorr = ShakerCorr(duration, amplitude, frequency);
        StartCoroutine(camShakeCorr);
    }

    #endregion


    private void Awake()
    {
        Instance = this;
    }

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
        camShakeCorr = null;
    }   
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
            Shake();

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

        rotatedLookDir = Quaternion.Euler(0, target.localEulerAngles.y, 0) * charControl.lookDir;
        //move cam
        desiredPos = target.transform.position - (rotatedLookDir * targetCamDist);
        transform.position = target.transform.position - (rotatedLookDir * curCamDist);
        //rotate cam
        transform.rotation = Quaternion.LookRotation(rotatedLookDir, Vector3.up);

        //crosshair.position = cam.ScreenToWorldPoint(new Vector3(Screen.width / 4, Screen.height / 4, cam.nearClipPlane));
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
#if UNITY_EDITOR
        Debug.DrawLine(clipPoint[0], target.transform.position, Color.white);
        Debug.DrawLine(clipPoint[1], target.transform.position, Color.white);
        Debug.DrawLine(clipPoint[2], target.transform.position, Color.white);
        Debug.DrawLine(clipPoint[3], target.transform.position, Color.white);
        Debug.DrawLine(clipPoint[4], target.transform.position, Color.green);
#endif
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

    private IEnumerator ShakerCorr(float duration, float amp, float freq)
    {
        float timer = 0;
        Transform lookAt = nextTarget;
        nextTarget = target;
        Vector3 up = transform.up;
        Vector3 right = Vector3.Cross(transform.forward, up);
        while (timer < duration)
        {
            timer += Time.deltaTime / freq;
            target.position += right * Random.Range(-amp, amp);
            target.position += up * Random.Range(-amp, amp);
            yield return null;
        }
        nextTarget = lookAt;
        camShakeCorr = null;
    }
}
