using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;

public class DestructibleController : MonoBehaviourPun
{
    /* TODO: Aim Assist */

    [Header("Pulling")]
    [SerializeField]
    [Tooltip("The layer mask used for detection of objects that can be picked up")]
    private LayerMask detectMask = 0;

    [SerializeField]
    [Tooltip("The furthest distance the player can pull objects from")]
    private float pickUpMaxDistance = 6.0f;

    [SerializeField]
    [Tooltip("The detection area around the object that is pulled")]
    private float detectionRadius = 4.5f;

    [SerializeField]
    [Tooltip("The speed at which objects are pulled towards the player")]
    private float pullSpeed = 1.0f;

    [Header("Holding")]
    [SerializeField]
    [Tooltip("The layer mask applied to throwables that are currently being held")]
    private LayerMask heldMask = 0;

    [SerializeField]
    [Tooltip("The maximum distance a pulled object can be away from the player to be considered being held")]
    private float holdPosTolerance = 1.0f;

    [SerializeField]
    [Tooltip("The speed at which the held object rotates at")]
    private float holdRotateSpeed = 2.0f;

    [SerializeField]
    [Tooltip("The maximum tolerance that the picked up object can be from the holding position (X axis)")]
    private Vector2 holdPosOffsetX = new Vector2(-0.25f, 0.90f);

    [SerializeField]
    [Tooltip("The maximum tolerance that the picked up object can be from the holding position (Y axis)")]
    private Vector2 holdPosOffsetY = new Vector2(-0.10f, 0.80f);

    [Header("Throwing")]
    [SerializeField]
    [Tooltip("The force applied to objects when pushing it away")]
    private float throwForce = 40.0f;

    [SerializeField]
    [Tooltip("Enable aim assist such that throwables are thrown towards the center crosshair")]
    private bool enableAimAssist = true;

    [SerializeField]
    [Tooltip("The layer mask used to aim at objects")]
    private LayerMask aimMask = 0;

    [SerializeField]
    [Tooltip("The minimum offset for aim assist to allow for more realistic accuracy")]
    private Vector3 aimAssistOffsetMin = new Vector3(-0.50f, -0.50f, -0.50f);

    [SerializeField]
    [Tooltip("The maximum offset for aim assist to allow for more realistic accuracy")]
    private Vector3 aimAssistOffsetMax = new Vector3(0.50f, 0.50f, 0.50f);

    [Header("Highlighting")]
    [SerializeField]
    [Tooltip("The material used to highlight objects taht can be picked up")]
    private Material highlightMaterial;

    /* A list of colliders of objects that are being pulled/thrown */
    private List<Collider> throwables = null;
    /* The offset positions of which the objects are held at */
    private List<Vector3> holdPositions;
    /* The default materials of objects prior to being highlighted */
    private Dictionary<GameObject, List<Material>> prevHighlighted = null;

    /* Event Callbacks for Crosshair */

    public event Action<bool> pullStatus = null;

    public event Action throwStatus = null;

    private CharTPController playerController = null;
    private Transform cameraTransform = null;
    private Transform holdDestructibles = null;

    private int heldMaskLayer = -1;
    private int detectMaskLayer = -1;
    private float holdPosToleranceSq = 0.0f;

    private const string holdDestructiblesTag = "HoldDestructibles";
    private const string heldMaskName = "HeldThrowables";
    private const string detectMaskName = "Throwable";

    private bool isPulling = false;
    private bool canThrow = false;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;

        heldMaskLayer = LayerMask.NameToLayer(heldMaskName);
        if (heldMaskLayer == -1)
            Debug.LogError("The layer " + heldMaskName + " is not set up in this project!");

        detectMaskLayer = LayerMask.NameToLayer(detectMaskName);
        if (detectMaskLayer == -1)
            Debug.LogError("The layer " + detectMaskName + " is not set up in this project!");

        StartCoroutine(initPlayer());

        holdPositions = new List<Vector3>();
        prevHighlighted = new Dictionary<GameObject, List<Material>>();
        holdPosToleranceSq = holdPosTolerance * holdPosTolerance;
    }
    private IEnumerator initPlayer()
    {
        while (GameManager.playerObj == null)
            yield return null;

        cameraTransform = Camera.main.transform;
        playerController = GameManager.playerObj.GetComponent<CharTPController>();
        if (playerController != null)
            holdDestructibles = playerController.transform.Find("BaseChar").Find("HoldDestructibles");
    }

    private void OnDestroy()
    {
        //Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (playerController == null)
            return;

        // Remove any destructibles currently holding that are owned by another player (other player reached destructible before us)
        if (throwables?.Count > 0)
        {
            for (int i = 0; i < throwables.Count; ++i)
            {
                if (throwables[i] == null)
                    continue;

                PhotonView view = PhotonView.Get(throwables[i]);
                if (view.Owner != null && view.Owner.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    // Requiring hold positions' to have the same index as throwables'
                    if (i < holdPositions.Count)
                        holdPositions.RemoveAt(i);
                    throwables.RemoveAt(i--);
                }
            }
            if (throwables.Count == 0)
            {
                // Reset player states
                isPulling = false;
                playerController.disableKeyInput = false;
                canThrow = false;
            }
        }

        RaycastHit hit;
        bool hasTarget = false;

        if (!isPulling && !canThrow)
        {
            hasTarget = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, pickUpMaxDistance + (cameraTransform.position - playerController.transform.position).magnitude, detectMask);

            /* Resets all previously highlighted materials if not pulling an object /preparing to throw */
            foreach (KeyValuePair<GameObject, List<Material>> pair in prevHighlighted)
            {
                if (pair.Key == null) continue;
                MeshRenderer meshRenderer = pair.Key.GetComponent<MeshRenderer>();
                meshRenderer.materials = pair.Value.ToArray();
            }
            prevHighlighted.Clear();

            /* Can pick up some throwables */
            if (hasTarget)
            {
                Collider[] collisions;
                collisions = Physics.OverlapSphere(hit.transform.position, detectionRadius, detectMask);
                throwables = new List<Collider>(collisions);

                /* Gets all surrounding throwables */
                if (throwables?.Count > 0)
                {
                    for (int i = 0; i < throwables.Count; ++i)
                    {
                        GameObject go = throwables[i].gameObject;
                        MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();

                        if (prevHighlighted.ContainsKey(go))
                            continue;

                        if (PhotonView.Get(go).Owner != null)
                        {
                            if (PhotonView.Get(go).Owner?.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                                continue;
                        }

                        /* Store current materials for reset later */
                        prevHighlighted.Add(go, meshRenderer.materials.OfType<Material>().ToList());

                        /* Switch to highlighted material */
                        Material[] highlightedMaterials = new Material[meshRenderer.materials.Length];
                        for (int j = 0; j < meshRenderer.materials.Length; ++j)

                            highlightedMaterials[j] = highlightMaterial;
                        meshRenderer.materials = highlightedMaterials;
                    }
                }
            }
        }

        // Start pulling
        if (Input.GetMouseButtonDown(0))
        {
            if (!isPulling)
            {
                if (hasTarget)
                {
                    // Don't grab objects that are already owned by other players
                    if (throwables.Count > 0)
                        for (int i = 0; i < throwables.Count; ++i)
                            if (PhotonView.Get(throwables[i]).Owner != null)
                                throwables.RemoveAt(i--);

                    if (throwables.Count > 0)
                    {
                        holdPositions.Clear();
                        for (int i = 0; i < throwables.Count; ++i)
                            setupThrowable(throwables[i]);

                        isPulling = true;
                        playerController.disableKeyInput = true;
                        pullStatus?.Invoke(isPulling);
                        // audio
                        playerController.GetComponent<ThrowableAudioController>()?.PickUpDebris();
                    }
                }
                else
                {
                    throwables = null;
                }
            }
        }

        // Release the destructibles
        if (Input.GetMouseButtonUp(0))
        {
            if (throwables?.Count > 0)
            {
                if (!canThrow)
                {
                    foreach (Collider collider in throwables)
                    {
                        if (collider == null) continue;

                        // Tell master client to release ownership back to scene
                        // Do not release locally as we might not have gotten the message that this object is now controlled by us before we release it
                        PhotonView colliderView = PhotonView.Get(collider);
                        photonView.RPC("destructibleReleaseOwner", RpcTarget.MasterClient, colliderView.ViewID, Vector3.zero);
                    }
                    Debug.Log("Reset throwables");
                    isPulling = false;
                    pullStatus?.Invoke(isPulling);
                    playerController.GetComponent<ThrowableAudioController>()?.DropDebris();
                }
                else
                {
                    RaycastHit[] aimHits;

                    Vector3 hitPoint = Vector3.zero;

                    /* Make held objects fly towards target object */
                    if (enableAimAssist)
                    {
                        aimHits = Physics.RaycastAll(cameraTransform.position, cameraTransform.forward, 50.0f, aimMask);

                        if (aimHits.Length > 0)
                        {
                            System.Array.Sort(aimHits, (x, y) => x.distance.CompareTo(y.distance));
                            for (int i = 0; i < aimHits.Length; ++i)
                            {
                                if (aimHits[i].transform == playerController.transform) continue;
                                hitPoint = aimHits[i].point;
                                break;
                            }
                        }
                    }

                    foreach (Collider collider in throwables)
                    {
                        if (collider == null) continue;

                        Vector3 targetDir;
                        if (!hitPoint.Equals(Vector3.zero))
                        {
                            Vector3 offset = hitPoint - collider.transform.position;

                            /* Add slight variations for aim assist fairness */
                            offset.x += UnityEngine.Random.Range(aimAssistOffsetMin.x, aimAssistOffsetMax.x);
                            offset.y += UnityEngine.Random.Range(aimAssistOffsetMin.y, aimAssistOffsetMax.y);
                            offset.z += UnityEngine.Random.Range(aimAssistOffsetMin.z, aimAssistOffsetMax.z);
                            targetDir = offset.normalized;
                        }
                        else
                        {
                            targetDir = cameraTransform.forward;
                        }

                        // Tell master client to release ownership back to scene
                        PhotonView colliderView = PhotonView.Get(collider);
                        collider.gameObject.layer = detectMaskLayer;
                        collider.attachedRigidbody.isKinematic = false;
                        collider.attachedRigidbody.useGravity = true;
                        NetworkOwnership.instance.releaseOwnership(colliderView, null, null);
                        photonView.RPC("destructibleReleaseOwner", RpcTarget.MasterClient, colliderView.ViewID, targetDir * throwForce);

                        //enable DamageData if have
                        DamageData damageData = collider.gameObject.GetComponent<DamageData>();
                        if (damageData) damageData.SetIsDamaging();
                    }

                    Debug.Log("Throw!");
                    canThrow = false;
                    throwStatus?.Invoke();
                    // audio
                    playerController.GetComponent<ThrowableAudioController>()?.LaunchDebris();
                }
                playerController.disableKeyInput = false;
            }
        }

        // Updating positions/rotations
        if (Input.GetMouseButton(0))
        {
            if (throwables?.Count > 0)
            {
                for (int i = 0; i < throwables.Count; ++i)
                {
                    Collider collider = throwables[i];
                    if (collider == null || i >= holdPositions.Count) continue;

                    Transform t = collider.transform;
                    Rigidbody rb = collider.attachedRigidbody;

                    /* Position over time */
                    Vector3 targetPos = holdDestructibles.position;
                    targetPos += holdDestructibles.right * holdPositions[i].x;
                    targetPos += holdDestructibles.up * holdPositions[i].y;
                    t.position = Vector3.Slerp(t.position, targetPos, Time.deltaTime * pullSpeed);

                    /* Rotation over time */
                    Vector3 rot = t.rotation.eulerAngles;
                    rot.z -= 10.0f;
                    rot.y += 4.0f;
                    t.rotation = Quaternion.Slerp(t.rotation, Quaternion.Euler(rot), Time.deltaTime * holdRotateSpeed);

                    /* Detects when the pulled object is near enough to be considered "held" */
                    float sqrDistFromHolding = (t.position - holdDestructibles.position).sqrMagnitude;
                    if (sqrDistFromHolding <= holdPosTolerance)
                    {
                        isPulling = false;
                        canThrow = true;
                    }
                }
            }
        }
    }

    private void setupThrowable(Collider collider)
    {
        if (PhotonNetwork.IsMasterClient)
            NetworkOwnership.instance.transferOwnerAsMaster(PhotonView.Get(collider), PhotonNetwork.LocalPlayer);
        else
            photonView.RPC("destructibleRequestOwner", RpcTarget.MasterClient, PhotonView.Get(collider).ViewID);

        collider.attachedRigidbody.useGravity = false;
        collider.attachedRigidbody.isKinematic = true;
        collider.gameObject.layer = heldMaskLayer;

        Vector3 targetPos = Vector3.zero;
        targetPos.x = UnityEngine.Random.Range(holdPosOffsetX.x, holdPosOffsetX.y);
        targetPos.y = UnityEngine.Random.Range(holdPosOffsetY.x, holdPosOffsetY.y);

        holdPositions.Add(targetPos);
    }

    [PunRPC]
    private void destructibleRequestOwner(int viewID, PhotonMessageInfo messageInfo)
    {
        PhotonView view = PhotonView.Find(viewID);
        if (view.Owner == null)
            NetworkOwnership.instance.transferOwnerAsMaster(view, messageInfo.Sender);
    }

    [PunRPC]
    private void destructibleReleaseOwner(int viewID, Vector3 force, PhotonMessageInfo messageInfo)
    {
        //Debug.Log("Received release request");
        PhotonView view = PhotonView.Find(viewID);
        if (view.Owner == null || view.Owner?.ActorNumber == messageInfo.Sender.ActorNumber)
        {
            view.TransferOwnership(0);
            view.gameObject.layer = detectMaskLayer;
            Rigidbody rb = view.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.Impulse);
            rb.WakeUp();
        }
        photonView.RPC("destructibleEnsureOwnerIsReleased", messageInfo.Sender, viewID);
    }

    [PunRPC]
    private void destructibleEnsureOwnerIsReleased(int viewID, PhotonMessageInfo messageInfo)
    {
        PhotonView view = PhotonView.Find(viewID);
        //Debug.Log("Ensure owner released called. View Owner " + view.Owner?.ActorNumber + " Player " + PhotonNetwork.LocalPlayer.ActorNumber);
        if (view.Owner?.ActorNumber == messageInfo.Sender.ActorNumber)
        {
            view.TransferOwnership(0);
            view.gameObject.layer = detectMaskLayer;
            Rigidbody rb = view.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.WakeUp();
        }
    }
}