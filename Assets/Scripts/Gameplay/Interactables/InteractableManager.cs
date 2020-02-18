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

    private bool interactDown = false;

    private void Update()
    {
        bool hasClickedInteract = !interactDown && Input.GetAxisRaw("Interact") > 0.5f;
        interactDown = Input.GetAxisRaw("Interact") > 0.5f;


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

        if (carryingInteractable == null && lastCollidedInteractable != null && hasClickedInteract) // Pickup the item currently looked at
        {
            carryingInteractable = lastCollidedInteractable.GetComponentInParent<InteractableBase>();

            if (!carryingInteractable.CanCarry) // Use the item if cannot carry
                carryingInteractable.interact();
            else // Can carry, attach to the player
            {
                Debug.Log("Started carrying");
                carryingInteractable.transform.parent = GameManager.playerObj.transform;
                carryingInteractable.transform.localPosition = carryingInteractable.PositionOffsetWhileCarry;
            }
        }
        else if (carryingInteractable != null) // Is currently carrying the item
        {
            if (Input.GetAxisRaw("UseInteractable") > 0.5f) // Use the item
                carryingInteractable.interact();
            else if (hasClickedInteract) // Drop the item
            {
                Debug.Log("Dropped the item");
                carryingInteractable.transform.parent = null;
                carryingInteractable = null;
            }
        }
    }

}
