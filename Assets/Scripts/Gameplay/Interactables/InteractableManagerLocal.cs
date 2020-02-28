using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class InteractableManagerLocal : MonoBehaviour
{
    [SerializeField]
    private Camera camera;

    [SerializeField]
    private float maxInteractDist = 10.0f;
    [SerializeField]
    private float interactRaycastLostTimeout = 0.4f;

    [SerializeField]
    private TextMeshProUGUI interactableTooltip;

    private InteractableBase lastCollidedInteractable = null;
    private float lastCollidedTimer = 0.0f;
    private InteractableBase carryingInteractable = null;

    private bool hasStoppedUsingInteractable = false;
    private InteractableBase usingInteractable = null;

    private bool interactDown = false;

    private void Update()
    {
        if (GameManager.playerObj == null)
            return;

        bool hasClickedInteract = !interactDown && Input.GetAxisRaw("Interact") > 0.5f;
        interactDown = Input.GetAxisRaw("Interact") > 0.5f;


        // Detect if looking at an interactable

        lastCollidedTimer += Time.deltaTime;

        Ray camRay = new Ray(camera.transform.position, camera.transform.forward);
        LayerMask mask = LayerMask.GetMask("Interactable");
        RaycastHit rayInfo;
        if (Physics.Raycast(camRay, out rayInfo, maxInteractDist + (camera.transform.position - GameManager.playerObj.transform.position).magnitude, mask))
        {
            if (rayInfo.collider.isTrigger)
            {
                //if (lastCollidedInteractable == null || rayInfo.collider.gameObject == lastCollidedInteractable.gameObject)
                //{
                    lastCollidedInteractable = rayInfo.collider.gameObject.GetComponentInParent<InteractableBase>();
                    lastCollidedTimer = 0.0f;
                //}
            }
        }
        // If not looking at an interactable, disallow interaction if last seen interactable is not lenient
        else if (lastCollidedInteractable != null && !lastCollidedInteractable.LenientInteraction)
            lastCollidedInteractable = null;

        // Time-out with last seen interactable, if not looking at an interactable
        if (lastCollidedTimer > interactRaycastLostTimeout)
            lastCollidedInteractable = null;


        // Pickup/Use looked at interactable

        if (usingInteractable != null) // Currently using an interactable
        {
            if (!hasStoppedUsingInteractable) // Don't send release request more than once
            {
                if (interactDown && !usingInteractable.interactDone) // Player is still interacting
                    usingInteractable.interact();
                else
                {
                    hasStoppedUsingInteractable = true;
                    InteractableManagerMaster.instance.releaseConstantUseInteractable(usingInteractable, releaseConstantUseInteractable, null);
                }
            }
        }
        else if (carryingInteractable == null && lastCollidedInteractable != null && hasClickedInteract) // Pickup the item currently looked at
        {
            if (lastCollidedInteractable.AllowConstantInteraction) // The interactable must be constantly interacted with
                InteractableManagerMaster.instance.constantUseInteractable(lastCollidedInteractable, constantUseInteractable, null);
            else if (lastCollidedInteractable.CanCarry) // The interactable can be carried
                InteractableManagerMaster.instance.carryInteractable(lastCollidedInteractable, carryInteractable, null);
            else // The interactable must be used immediately
                InteractableManagerMaster.instance.useInteractable(lastCollidedInteractable, useInteractable, null);
        }
        else if (carryingInteractable != null) // Is currently carrying the item
        {
            if (Input.GetAxisRaw("UseInteractable") > 0.5f) // Use the item
                InteractableManagerMaster.instance.useInteractable(carryingInteractable, useInteractable, null);
            else if (hasClickedInteract) // Drop the item
                InteractableManagerMaster.instance.dropInteractable(carryingInteractable, dropInteractable, null);
        }

        // Show tooltip
        if (interactableTooltip != null)
        {
            if (carryingInteractable != null)
                interactableTooltip.text = carryingInteractable.getCarriedTooltip();
            else if (lastCollidedInteractable != null)
                interactableTooltip.text = lastCollidedInteractable.getUncarriedTooltip();
            else
                interactableTooltip.text = "";
        }
    }

    private void useInteractable(InteractableBase interactable)
    {
        interactable.interact();
        carryingInteractable = null;
    }
    private void constantUseInteractable(InteractableBase interactable)
    {
        usingInteractable = interactable;
    }
    private void releaseConstantUseInteractable(InteractableBase interactable)
    {
        NetworkOwnership.instance.releaseOwnership(PhotonView.Get(interactable), null, null);
        usingInteractable = null;
        hasStoppedUsingInteractable = false;
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
