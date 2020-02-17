using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharTPController : MonoBehaviour
{

    public Camera cam;
    public float moveSpeed = 5;
    public float mouseSens = 1;
    public float maxLookY = 0.7f;

    //calculated values
    private Vector3 lookDir;
    private Vector3 moveAmt;
    private float adjustedOffset; //TODO

    //these are just to cache values
    private Vector3 forward, right;
    private float camOffset;
    private float mouseInputY;

    private void Start()
    { 
        lookDir = transform.position - cam.transform.position;
        lookDir.Set(lookDir.x, Mathf.Clamp(lookDir.y, -maxLookY, maxLookY), lookDir.z);
        forward = new Vector3(lookDir.x, 0, lookDir.z);
        camOffset = lookDir.magnitude;
        lookDir.Normalize();
        forward.Normalize();

        adjustedOffset = camOffset;

        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        //check y
        mouseInputY = Input.GetAxis("Mouse Y");
        if ((lookDir.y > maxLookY && mouseInputY > 0) || (lookDir.y < -maxLookY && mouseInputY < 0))
            mouseInputY = 0;
        //calculate where cam and player is facing
        lookDir = Quaternion.Euler(-mouseInputY * mouseSens, Input.GetAxis("Mouse X") * mouseSens, 0) * lookDir;
        lookDir.Normalize();
        //remove y for movement
        forward.Set(lookDir.x, 0, lookDir.z);
        forward.Normalize();


        //move player
        right = Vector3.Cross(Vector3.up, forward);
        moveAmt = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
        //rotate player
        transform.LookAt(transform.position + forward);

        //move cam
        cam.transform.position = transform.position - (lookDir * adjustedOffset);
        //rotate cam
        cam.transform.LookAt(transform.position);
    }

    private void FixedUpdate()
    {        
        transform.position += moveAmt.normalized * Time.deltaTime * moveSpeed;    
    }
}
