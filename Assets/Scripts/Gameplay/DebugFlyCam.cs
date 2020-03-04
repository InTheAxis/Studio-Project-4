using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class DebugFlyCam : MonoBehaviour
{
    [SerializeField]
    private CharTPCamera tpCam;

    [SerializeField]
    private float startSpeed = 10;
    [SerializeField]
    private float scrollScale = 0.2f;

    private float speed;
    private Camera cam;
    private bool isEnabled;
    private Vector3 lookDir;

    private float mouseX, mouseY;

    private void Awake()
    {
        speed = startSpeed;
        cam = GetComponent<Camera>();
        cam.enabled = isEnabled = false;
        lookDir = transform.forward;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            isEnabled = !isEnabled;
            cam.enabled = isEnabled;
            if (tpCam)
            { 
                tpCam.gameObject.SetActive(!isEnabled);
                tpCam.charControl.DisableKeyInput(isEnabled);
                tpCam.charControl.DisableMouseInput(isEnabled);
                transform.position = tpCam.transform.position;
            }
        }

        if (isEnabled)
        {
            if (Input.GetKey(KeyCode.W))
                transform.position += transform.forward * Time.deltaTime * speed;
            if (Input.GetKey(KeyCode.S))
                transform.position -= transform.forward * Time.deltaTime * speed;
            if (Input.GetKey(KeyCode.D))
                transform.position += transform.right * Time.deltaTime * speed;
            if (Input.GetKey(KeyCode.A))
                transform.position -= transform.right * Time.deltaTime * speed;
            if (Input.GetKey(KeyCode.Q))
                transform.position += Vector3.up * Time.deltaTime * speed;
            if (Input.GetKey(KeyCode.E))
                transform.position -= Vector3.up * Time.deltaTime * speed;

            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");
            lookDir = Quaternion.Euler(0, mouseX, 0) * lookDir;
            lookDir = Quaternion.AngleAxis(-mouseY, transform.right) * lookDir;
            transform.rotation = Quaternion.LookRotation(lookDir);

            speed = Mathf.Clamp(speed +  Input.mouseScrollDelta.y * scrollScale, 0, 100);

        }
    }
}
