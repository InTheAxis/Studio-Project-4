using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class InteractableManagerLocal : MonoBehaviour
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
            InteractableBase lastCollided = lastCollidedInteractable.GetComponentInParent<InteractableBase>();
            if (lastCollided.CanCarry) // The interactable can be carried
                InteractableManagerMaster.instance.carryInteractable(lastCollided, carryInteractable, null);
            else // The interactable must be used immediately
                InteractableManagerMaster.instance.useInteractable(lastCollided, useInteractable, null);
        }
        else if (carryingInteractable != null) // Is currently carrying the item
        {
            if (Input.GetAxisRaw("UseInteractable") > 0.5f) // Use the item
                InteractableManagerMaster.instance.useInteractable(carryingInteractable, useInteractable, null);
            else if (hasClickedInteract) // Drop the item
                InteractableManagerMaster.instance.dropInteractable(carryingInteractable, dropInteractable, null);
        }
    }

    private void useInteractable(InteractableBase interactable)
    {
        interactable.interact();
        carryingInteractable = null;
        NetworkOwnership.instance.destroy(PhotonView.Get(interactable));
    }
    private void carryInteractable(InteractableBase interactable)
    {
        carryingInteractable = interactable;
        interactable.transform.parent = GameManager.playerObj.transform;
        interactable.transform.localPosition = interactable.PositionOffsetWhileCarry;
    }
    private void dropInteractable(InteractableBase interactable)
    {
        NetworkOwnership.instance.releaseOwnership(PhotonView.Get(interactable), null, null);
        interactable.transform.parent = null;
        carryingInteractable = null;
    }
}
