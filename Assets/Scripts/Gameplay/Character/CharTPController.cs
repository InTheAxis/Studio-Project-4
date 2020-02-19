using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharTPController : MonoBehaviourPun
{
    public List<Transform> lookTargets; //places for cam to look
    public CharJumpCheck jumpChk;
    public CharCrouchCheck crouchChk;

    [Tooltip("Vertical and Horizontal movement speed")]
    public float moveSpeed = 5;
    [Tooltip("Mouse sensitivity, affects rotation speed")]
    public float mouseSens = 1;
    [Tooltip("Clamps how far you can look up or down")]
    public float maxLookY = 0.6f;
    [Tooltip("Starting y-axis look direction, from -1 to 1")]
    public float initialLookY = 0.8f;

    [Tooltip("Higher means able to move more in air")]
    public float airSpeed = 1;
    [Tooltip("Speed of crouch walk")]
    public float crouchSpeed = 2;
    [Tooltip("Force applied to rigidbody to jump upwards")]
    public float jumpForce = 2;

    [HideInInspector]
    public bool disableMovement;
    [HideInInspector]
    public bool disableKeyInput;
    [HideInInspector]
    public bool disableMouseInput;
    public float velY { private set; get; }
    public float displacement { private set; get; } //how fast im moving xz
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
        disableMovement = false;
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;
        if (!disableKeyInput)
        {
            inp.vert = Input.GetAxisRaw("Vertical");
            inp.hori = Input.GetAxisRaw("Horizontal");
            inp.crouch = Input.GetAxisRaw("Crouch") != 0;
            inp.jump = Input.GetAxisRaw("Jump") != 0;
        }
        if (!disableMouseInput)
        { 
            inp.mouseY = Input.GetAxis("Mouse Y");
            inp.mouseX = Input.GetAxis("Mouse X");
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;

        //check y
        if ((lookDir.y > maxLookY && inp.mouseY > 0) || (lookDir.y < -maxLookY && inp.mouseY < 0))
            inp.mouseY = 0;
        //calculate where cam and player is facing  
        lookDir = Quaternion.Euler(0, inp.mouseX * mouseSens, 0) * lookDir;
        lookDir = Quaternion.AngleAxis(-inp.mouseY * mouseSens, right) * lookDir;
        lookDir.Normalize();

        if (!disableMovement)
        {
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

            velY = rb.velocity.y;
            displacement = moveAmt.magnitude * currSpeed;

            if (velY > 0)
                jumpChk.Jumping();
            if (inp.jump && !jumpChk.airborne)            
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);            

            //move player
            transform.position += moveAmt * Time.deltaTime * currSpeed;
            //rotate player
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        rb.velocity = new Vector3(0, rb.velocity.y, 0); //reset in case it slides    
    }
}
