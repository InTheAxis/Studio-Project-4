using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharTPController : MonoBehaviour
{
    public CharJumpCheck jumpChk;
    public CharCrouchCheck crouchChk;

    public float moveSpeed = 5;
    public float mouseSens = 1;
    public float maxLookY = 0.6f;
    public float initialLookY = 0.8f;

    public float airSpeed = 1;
    public float crouchSpeed = 2;
    public float jumpForce = 2;

    public Vector3 lookDir { private set; get; }

    //these are just to cache values
    private Rigidbody rb;
    private Vector3 forward, right;
    private float currSpeed;
    private Vector3 moveAmt;

    private string state;

    private struct InputData
    {
        public float vert, hori;
        public float mouseY, mouseX;
        public bool jump, crouch;
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

        state = "idle";
    }
    private void Update()
    {

        inp.vert = Input.GetAxisRaw("Vertical");
        inp.hori = Input.GetAxisRaw("Horizontal");
        inp.mouseX = Input.GetAxis("Mouse X");
        inp.mouseY = Input.GetAxis("Mouse Y");
        inp.crouch = Input.GetAxis("Crouch") != 0;
        inp.jump = Input.GetAxis("Jump") != 0;
    }

    private void FixedUpdate()
    {
        //check y
        if ((lookDir.y > maxLookY && inp.mouseY > 0) || (lookDir.y < -maxLookY && inp.mouseY < 0))
            inp.mouseY = 0;
        //calculate where cam and player is facing  
        lookDir = Quaternion.Euler(0, inp.mouseX * mouseSens, 0) * lookDir;
        lookDir = Quaternion.AngleAxis(-inp.mouseY * mouseSens, right) * lookDir;
        lookDir.Normalize();
        //remove y for movement
        forward.Set(lookDir.x, 0, lookDir.z);
        forward.Normalize();
        right = Vector3.Cross(Vector3.up, forward);

        crouchChk.Crouch(inp.crouch && !jumpChk.airborne);
        if (crouchChk.crouching)
            currSpeed = crouchSpeed;
        else if (jumpChk.airborne)
            currSpeed = airSpeed;
        else
            currSpeed = moveSpeed;

        moveAmt = (forward * inp.vert + right * inp.hori).normalized;

        if (inp.jump && !jumpChk.airborne)
        {
            jumpChk.Jumped();
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        //move player
        transform.position += moveAmt * Time.deltaTime * currSpeed;
        //rotate player
        transform.LookAt(transform.position + forward);

        CalculateState();
    }


    private void CalculateState()
    {
        if (jumpChk.airborne)
        {
            if (rb.velocity.y > 0)
            {
                if (rb.velocity.y < jumpForce)
                    state = "hang";
                else
                    state = "jump";
            }
            else
                state = "fall";
        }
        else if (crouchChk.crouching)
        {
            state = "crouch";
        }
        else if (moveAmt.magnitude > 0)
        {
            state = "run";
        }
        else
            state = "idle";

    }
}
