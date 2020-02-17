using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharTPController : MonoBehaviour
{
    public CharTPCrouch crouch;
    public CharTPJump jump;

    public float moveSpeed = 5;
    public float mouseSens = 1;
    public float maxLookY = 0.6f;
    public float initialLookY = 0.8f;

    public float crouchSpeed = 2;
    public float jumpForce = 2;

    public Vector3 lookDir { private set; get; }

    //these are just to cache values
    private Rigidbody rb;
    private Vector3 forward, right;
    private float currSpeed;
    private Vector3 moveAmt;

    private struct InputData
    {
        public float vert, hori;
        public float mouseY, mouseX;
    };
    private InputData inp;

    private void Start()
    {
        initialLookY = Mathf.Clamp(initialLookY, -maxLookY, maxLookY);

        forward = transform.forward;
        forward.y = 0;
        lookDir = new Vector3(forward.x, initialLookY, forward.z).normalized;
        forward.Normalize();

        currSpeed = moveSpeed;

        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {

        inp.vert = Input.GetAxis("Vertical");
        inp.hori = Input.GetAxis("Horizontal");
        inp.mouseX = Input.GetAxis("Mouse X");
        inp.mouseY = Input.GetAxis("Mouse Y");
        crouch.SetInput(Input.GetAxis("Crouch") != 0);
        jump.SetInput(Input.GetAxis("Jump") != 0);
    }

    private void FixedUpdate()
    {
        //check y
        if ((lookDir.y > maxLookY && inp.mouseY > 0) || (lookDir.y < -maxLookY && inp.mouseY < 0))
            inp.mouseY = 0;
        //calculate where cam and player is facing  
        lookDir = Quaternion.Euler(-inp.mouseY * mouseSens, inp.mouseX * mouseSens, 0) * lookDir;
        lookDir.Normalize();
        //remove y for movement
        forward.Set(lookDir.x, 0, lookDir.z);
        forward.Normalize();

        currSpeed = crouch.crouching ? crouchSpeed : moveSpeed;

        moveAmt = (forward * inp.vert + right * inp.hori).normalized;

        if (jump.jumping)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        //move player
        right = Vector3.Cross(Vector3.up, forward);
        transform.position += moveAmt * Time.deltaTime * currSpeed;
        //rotate player
        transform.LookAt(transform.position + forward);
    }
}
