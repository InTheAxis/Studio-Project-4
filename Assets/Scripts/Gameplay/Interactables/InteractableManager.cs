using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableManager : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private float maxInteractDist = 50.0f;
    [SerializeField]
    private float interactRaycastLostTimeout = 0.4f;

    private GameObject lastCollidedInteractable = null;
    private float lastCollidedTimer = 0.0f;
    private InteractableBase carryingInteractable = null;

    private void Update()
    {
        // Detect if looking at an interactable
        
        lastCollidedTimer += Time.deltaTime;

        Ray camRay = new Ray(camera.transform.position, camera.transform.forward);
        LayerMask mask = LayerMask.GetMask("Interactable");
        RaycastHit rayInfo;
        if (Physics.Raycast(camRay, out rayInfo, maxInteractDist, mask))
        {
            if (rayInfo.collider.isTrigger)
            {
                if (lastCollidedInteractable == null || rayInfo.collider.gameObject == lastCollidedInteractable)
                {
                    lastCollidedInteractable = rayInfo.collider.gameObject;
                    lastCollidedTimer = 0.0f;
                }
            }
        }

        if (lastCollidedTimer > interactRaycastLostTimeout)
            lastCollidedInteractable = null;


        // Pickup/Use looked at interactable

        if (lastCollidedInteractable != null && Input.GetAxisRaw("Interact") > 0.5f)
        {
            carryingInteractable = lastCollidedInteractable.GetComponentInParent<InteractableBase>();
            if (carryingInteractable.CanCarry)
                Debug.Log("Carried");
            else
                carryingInteractable.interact();
        }
    }

}
