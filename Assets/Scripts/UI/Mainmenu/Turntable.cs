using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Turntable : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The mask used for shooting rays against")]
    private LayerMask uiMask;
    [SerializeField]
    [Tooltip("How much the model rotates while idle")]
    private float rotateAmt = 5.0f;
    [SerializeField]
    [Tooltip("How fast the model rotates to target angle")]
    private float rotateSpeed = 5.0f;
    [SerializeField]
    [Tooltip("How much the model rotates based on mouse drag")]
    private float turnSensitivity = 4.0f;
    [SerializeField]
    [Tooltip("Enable drag-to-rotate interactions")]
    private bool interactable = true;
    [SerializeField]
    [Tooltip("Auto-rotate the model")]
    private bool autoRotate = true;
    [SerializeField]
    [Tooltip("Rotate according to mouse movement")]
    private bool rotateToMouse = false;


    [SerializeField]
    private bool dragging = false;

    private void Update()
    {

        
        if (interactable)
        {
            /* Check if an object is under the cursor */
            if (Input.GetMouseButtonDown(0) && !isPointerOverUIElement())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                    dragging = true;
            }

            if (Input.GetMouseButtonUp(0))
                dragging = false;
        }


        Vector3 rotation = transform.rotation.eulerAngles;

        /* Turn table and auto rotate */
        if (!dragging && autoRotate)
            rotation.y += rotateAmt;
        /* Rotate according to mouse or drag to rotate */
        else if (rotateToMouse || (dragging && interactable))
            rotation.y -= Input.GetAxisRaw("Mouse X") * turnSensitivity;

        /* Apply rotation */
        Quaternion rot = Quaternion.Euler(rotation);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * rotateSpeed);
    }

    private bool isPointerOverUIElement()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
