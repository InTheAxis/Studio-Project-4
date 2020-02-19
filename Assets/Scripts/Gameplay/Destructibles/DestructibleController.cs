using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleController : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform = null;
    [SerializeField]
    private float detectionRadius = 4.5f;
    [SerializeField]
    private LayerMask layerMask = 0;

    [SerializeField]
    private Transform holdDestructibles = null;

    [SerializeField]
    private CharTPController playerController = null;
    private Collider[] throwables;
    private List<Vector3> holdPositions;
    private bool isPulling = false;
    private bool canThrow = false;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        holdPositions = new List<Vector3>();

        cameraTransform = Camera.main.transform;

        if (playerController == null)
            playerController = GetComponent<CharTPController>();
        //if (throwCollider == null)
        //    throwCollider = transform.Find("ThrowCheck").GetComponent<SphereCollider>();

        //throwCollider.radius = detectionRadius;


    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (!isPulling)
            {
                throwables = Physics.OverlapSphere(transform.position, detectionRadius, layerMask);
                if (throwables.Length > 0)
                {
                    holdPositions.Clear();
                    for(int i = 0; i < throwables.Length; ++i)
                    {
                        Collider collider = throwables[i];
                        collider.attachedRigidbody.useGravity = false;
                        //collider.attachedRigidbody.isKinematic = true;

                        Vector3 targetPos = holdDestructibles.position;
                        targetPos += transform.right * Random.Range(-0.25f, 0.90f);
                        targetPos.y += Random.Range(-0.10f, 0.80f);
                        holdPositions.Add(targetPos);
                    }
                    isPulling = true;
                    playerController.disableKeyInput = true;
                }
            }
        }


        if (Input.GetMouseButtonUp(0))
        {

            if (throwables.Length > 0)
            {
                if (!canThrow)
                {

                    foreach (Collider collider in throwables)
                    {
                        if (collider == null) continue;

                        collider.attachedRigidbody.useGravity = true;
                        collider.attachedRigidbody.isKinematic = false;

                    }
                    Debug.Log("Reset throwables");
                    isPulling = false;

                }
                else
                {
                    RaycastHit hit;
                    bool hasTarget = Physics.Raycast(cameraTransform.position + cameraTransform.forward * 5.0f, cameraTransform.forward, out hit, 100.0f);


                    if (hasTarget)
                        Debug.Log("Found target");
                    else
                        Debug.Log("no target");

                    foreach (Collider collider in throwables)
                    {
                        if (collider == null) continue;

                        Rigidbody rb = collider.attachedRigidbody;
                        rb.useGravity = true;
                        rb.isKinematic = false;

                        Vector3 throwDir;

                        if (hasTarget)
                        {
                            throwDir = (hit.point - collider.transform.position).normalized;
                        }
                        else
                        {
                            throwDir = cameraTransform.forward;
                        }
                        rb.AddForce(throwDir * 40.0f, ForceMode.Impulse);
                    }


                    Debug.Log("Throw!");
                    canThrow = false;
                }
                playerController.disableKeyInput = false;

            }
        }

        if (Input.GetMouseButton(0))
        {
            for (int i = 0; i < throwables.Length; ++i)
            {
                Collider collider = throwables[i];
                if (collider == null) continue;

                Transform t = collider.transform;
                Rigidbody rb = collider.attachedRigidbody;

                //Vector3 offset = holdDestructibles.position - t.position;
                //if(offset.sqrMagnitude > 2.0f)
                ////    rb.AddForce(offset.normalized * 5.0f, ForceMode.Acceleration);
                //Vector3 targetPos = transform.position;
                //targetPos += transform.right * holdPositions[i].x;
                //targetPos += transform.up * holdPositions[i].y;
                t.position = Vector3.Slerp(t.position, holdPositions[i], Time.deltaTime * 5.0f);
                //else
                //    rb.AddForce(offset.normalized * 5.0f, ForceMode.Acceleration);
                //t.position = Vector3.Lerp(t.position, holdDestructibles.position, Time.deltaTime * 5.0f);

                float sqrDistFromHolding = (holdPositions[i] - t.position).sqrMagnitude;
                if(sqrDistFromHolding <= 2.0f)
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
}
