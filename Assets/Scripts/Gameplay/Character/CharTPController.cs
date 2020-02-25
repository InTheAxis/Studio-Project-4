using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharTPController : MonoBehaviourPun
{
    [Header("References, look for them in children")]
    public List<Transform> lookTargets; //places for cam to look

    public CharJumpCheck jumpChk;
    public CharCrouchCheck crouchChk;

    [Header("Speed settings")]
    [SerializeField]
    [Tooltip("Vertical and Horizontal movement speed")]
    private float moveSpeed = 50;

    [SerializeField]
    [Tooltip("How fast to deccelerate after letting go of movement key")]
    private float deccel = 10;

    [SerializeField]
    [Tooltip("Higher means able to move more in air")]
    private float airMoveSpeed = 20;

    [SerializeField]
    [Tooltip("Speed of crouch walk")]
    private float crouchSpeed = 25;

    [SerializeField]
    [Tooltip("Speed of jump")]
    private float jumpSpeed = 5;

    [Header("Mouse Settings")]
    [SerializeField]
    [Tooltip("Mouse sensitivity, affects rotation speed")]
    private float mouseSens = 1;

    [SerializeField]
    [Tooltip("Clamps how far you can look up or down")]
    private float maxLookY = 0.9f;

    [SerializeField]
    [Tooltip("Starting y-axis look direction, from -1 to 1")]
    private float initialLookY = -0.5f;

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

    // List of all objs with CharTPController, updated in OnEnable and OnDisable, to be used for anywhere that needs references to other players
    private static List<CharTPController> playerControllerRefs = new List<CharTPController>();
    public static List<CharTPController> PlayerControllerRefs { get => playerControllerRefs; }

    private void OnEnable()
    {
        playerControllerRefs.Add(this);
    }
    private void OnDisable()
    {
        playerControllerRefs.Remove(this);
    }

    private void Start()
    {
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
            inp.mouseY = Input.GetAxisRaw("Mouse Y");
            inp.mouseX = Input.GetAxisRaw("Mouse X");
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
                currSpeed = airMoveSpeed;
            else
                currSpeed = moveSpeed;

            moveAmt = (pforward * inp.vert + right * inp.hori).normalized;
            moveAmt.y = 0;

            velY = rb.velocity.y;
            displacement = moveAmt.magnitude * currSpeed;

            if (velY > 0)
                jumpChk.Jumping();
            if (inp.jump && !jumpChk.airborne)
                rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);

            //move player
            //pPos = rb.position;
            //pPos += moveAmt * Time.deltaTime * currSpeed;
            //Move(pPos);
            rb.AddForce(moveAmt * currSpeed, ForceMode.Acceleration);
            //rotate player
            transform.rotation = Quaternion.LookRotation(pforward, Vector3.up);
        }

        //deccelerate to 0
        Vector3 temp = rb.velocity;
        temp = Vector3.Lerp(temp, Vector3.zero, Time.deltaTime * deccel);
        temp.y = rb.velocity.y;
        rb.velocity = temp;
    }

    public void AddForce(Vector3 force, ForceMode mode)
    {
        rb.AddForce(force, mode);
    }
}