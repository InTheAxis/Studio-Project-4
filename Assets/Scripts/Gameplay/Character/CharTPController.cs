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
    public Vector3 position { get { return transform.position; } }
    public Vector3 forward { get { return transform.forward; } }
    public float velY { private set; get; }
    public float displacement { private set; get; } //how fast im moving xz
    public Vector3 lookDir { private set; get; }

    //these are just to cache values
    private Rigidbody rb;
    private Vector3 pPos, pforward;
    private Vector3 right;
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
        if (CharTPCamera.Instance != null)        
            CharTPCamera.Instance.SetCharController(this);        

        initialLookY = Mathf.Clamp(initialLookY, -maxLookY, maxLookY);

        pPos = transform.position;
        pforward = transform.forward;
        pforward.y = 0;
        lookDir = new Vector3(pforward.x, initialLookY, pforward.z).normalized;
        pforward.Normalize();

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
        else 
        {
            inp.vert = 0;
            inp.hori = 0;
            inp.crouch = false;
            inp.jump = false;
        }
        if (!disableMouseInput)
        {
            inp.mouseY = Input.GetAxis("Mouse Y");
            inp.mouseX = Input.GetAxis("Mouse X");
        }
        else 
        {
            inp.mouseY = 0;
            inp.mouseX = 0;
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
            pforward.Set(lookDir.x, 0, lookDir.z);
            pforward.Normalize();
            right = Vector3.Cross(Vector3.up, pforward);


            crouchChk.Crouch(inp.crouch && !jumpChk.airborne);
            if (crouchChk.crouching)
                currSpeed = crouchSpeed;
            else if (jumpChk.airborne)
                currSpeed = airSpeed;
            else
                currSpeed = moveSpeed;

            moveAmt = (pforward * inp.vert + right * inp.hori).normalized;
            moveAmt.y = 0;

            velY = rb.velocity.y;
            displacement = moveAmt.magnitude * currSpeed;

            if (velY > 0)
                jumpChk.Jumping();
            if (inp.jump && !jumpChk.airborne)            
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            //move player
            pPos = rb.position;
            pPos += moveAmt * Time.deltaTime * currSpeed;
            Move(pPos);
            //rotate player
            transform.rotation = Quaternion.LookRotation(pforward, Vector3.up);
        }

        rb.velocity = new Vector3(0, rb.velocity.y, 0); //reset in case it slides    
    }

    public void Move(Vector3 pos)
    {
        rb.MovePosition(pos);
    }
}
