using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DestructibleController : MonoBehaviourPun
{
    [SerializeField]
    private Transform cameraTransform = null;
    [SerializeField]
    private float pickUpMaxDistance = 6.0f;
    [SerializeField]
    private float detectionRadius = 4.5f;
    [SerializeField]
    private LayerMask layerMask = 0;
    

    [SerializeField]
    private Transform holdDestructibles = null;

    [SerializeField]
    private CharTPController playerController = null;
    private List<Collider> throwables = null;
    private List<Vector3> holdPositions;
    private bool isPulling = false;
    private bool canThrow = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        holdPositions = new List<Vector3>();

        cameraTransform = Camera.main.transform;

        //if (playerController == null)
        //    playerController = GetComponent<CharTPController>();
        //if (throwCollider == null)
        //    throwCollider = transform.Find("ThrowCheck").GetComponent<SphereCollider>();

        playerController = GameManager.playerObj.GetComponent<CharTPController>();
        if (playerController != null)
            holdDestructibles = playerController.transform.Find("LookTarget").Find("HoldDestructibles");

        //throwCollider.radius = detectionRadius;
    }

    private void Update()
    {
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

        // Start pulling
        if (Input.GetMouseButtonDown(0))
        {
            if (!isPulling)
            {
                RaycastHit hit;
                bool hasTarget = Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, pickUpMaxDistance, layerMask);

                if (hasTarget)
                {
                    Collider[] collisions;
                    collisions = Physics.OverlapSphere(hit.transform.position, detectionRadius, layerMask);
                    throwables = new List<Collider>(collisions);

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

            if (throwables != null && throwables.Count > 0)
            {
                if (!canThrow)
                {

                    foreach (Collider collider in throwables)
                    {
                        if (collider == null) continue;

                        collider.attachedRigidbody.useGravity = true;
                        collider.attachedRigidbody.isKinematic = false;

                        // Tell master client to release ownership back to scene
                        // Do not release locally as we might not have gotten the message that this object is now controlled by us before we release it
                        PhotonView colliderView = PhotonView.Get(collider);
                        photonView.RPC("destructibleReleaseOwner", RpcTarget.MasterClient, colliderView.ViewID);

                    }
                    Debug.Log("Reset throwables");
                    isPulling = false;

                }
                else
                {
                    //RaycastHit hit;
                    //bool hasTarget = Physics.Raycast(cameraTransform.position + cameraTransform.forward * 5.0f, cameraTransform.forward, out hit, 100.0f);


                    //if (hasTarget)
                    //    Debug.Log("Found target");
                    //else
                    //    Debug.Log("no target");

                    foreach (Collider collider in throwables)
                    {
                        if (collider == null) continue;

                        Rigidbody rb = collider.attachedRigidbody;
                        rb.useGravity = true;
                        rb.isKinematic = false;

                        Vector3 throwDir;

                        //if (hasTarget)
                        //{
                        //    throwDir = (hit.point - collider.transform.position).normalized;
                        //}
                        //else
                        //{
                            throwDir = cameraTransform.forward;
                        //}
                        rb.AddForce(throwDir * 40.0f, ForceMode.Impulse);

                        // Tell master client to release ownership back to scene
                        // Do not release locally as we might not have gotten the message that this object is now controlled by us before we release it
                        PhotonView colliderView = PhotonView.Get(collider);
                        photonView.RPC("destructibleReleaseOwner", RpcTarget.MasterClient, colliderView.ViewID);
                    }


                    Debug.Log("Throw!");
                    canThrow = false;
                }
                playerController.disableKeyInput = false;

            }
        }

        // Updating positions/rotations
        if (Input.GetMouseButton(0) && throwables != null)
        {
            for (int i = 0; i < throwables.Count; ++i)
            {
                Collider collider = throwables[i];
                if (collider == null) continue;

                Transform t = collider.transform;
                Rigidbody rb = collider.attachedRigidbody;

                //Vector3 offset = holdDestructibles.position - t.position;
                //if(offset.sqrMagnitude > 2.0f)
                ////    rb.AddForce(offset.normalized * 5.0f, ForceMode.Acceleration);
                ///
                Vector3 targetPos = holdDestructibles.position;
                targetPos += holdDestructibles.right * holdPositions[i].x;
                targetPos += holdDestructibles.up * holdPositions[i].y;



                t.position = Vector3.Slerp(t.position, targetPos, Time.deltaTime * 2.0f);
                //else
                //    rb.AddForce(offset.normalized * 5.0f, ForceMode.Acceleration);
                //t.position = Vector3.Lerp(t.position, holdDestructibles.position, Time.deltaTime * 5.0f);

                float sqrDistFromHolding = (t.position - holdDestructibles.position).sqrMagnitude;
                if(sqrDistFromHolding <= 1.5f)
                {
                    isPulling = false;
                    canThrow = true;
                }
            }

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }


    private void setupThrowable(Collider collider)
    {
        if (PhotonNetwork.IsMasterClient)
            NetworkOwnership.instance.transferOwnerAsMaster(PhotonView.Get(collider), PhotonNetwork.LocalPlayer);
        else
            photonView.RPC("destructibleRequestOwner", RpcTarget.MasterClient, PhotonView.Get(collider).ViewID);

        collider.attachedRigidbody.useGravity = false;

        //Vector3 targetPos = holdDestructibles.position;
        //targetPos += transform.right * Random.Range(-0.25f, 0.90f);
        //targetPos.y += Random.Range(-0.10f, 0.80f);

        Vector3 targetPos = Vector3.zero;
        targetPos.x = Random.Range(-0.25f, 0.90f);
        targetPos.y = Random.Range(-0.10f, 0.80f);

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
    private void destructibleReleaseOwner(int viewID, PhotonMessageInfo messageInfo)
    {
        Debug.Log("Received release request");
        PhotonView view = PhotonView.Find(viewID);
        if (view.Owner?.ActorNumber == messageInfo.Sender.ActorNumber)
            view.TransferOwnership(0);
    }
}
