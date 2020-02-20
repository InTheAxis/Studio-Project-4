using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScreenStateController : MonoBehaviour
{

    [SerializeField]
    private float hoverGrowSize = 1.2f;

    private List<GameObject> hovered;
    private List<GameObject> prevHovered;
    private List<RaycastResult> raycastResults;

    private Vector3 hoverGrowScale = Vector3.zero;

    private void Start()
    {
        hovered = new List<GameObject>();
        prevHovered = new List<GameObject>();
        hoverGrowScale = new Vector3(hoverGrowSize, hoverGrowSize, hoverGrowSize);
    }

    private void Update()
    {
        //getHoveredUIElements();
        //if(raycastResults.Count > 0)
        //{
        //    hovered.Clear();
        //    foreach(RaycastResult result in raycastResults)
        //    {
        //        if(result.gameObject.CompareTag("Button"))
        //        {
        //            Transform t = result.gameObject.transform;
        //            t.localScale = hoverGrowScale;
        //            if (!hovered.Contains(result.gameObject))
        //                hovered.Add(result.gameObject);
        //        }
        //    }

        //    Debug.Log(prevHovered.Count);
        //    foreach(GameObject go in prevHovered)
        //    {
        //        //if(!hovered.Contains(go))
        //        //{
        //            Transform t = go.transform;
        //            t.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //        //}
        //    }

        //    prevHovered = hovered;



        //}
    }

    private void getHoveredUIElements()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
    }

    public void onHoverEnterButton(GameObject go)
    {
        go.transform.localScale = hoverGrowScale;
    }

    public void onHoverExitButton(GameObject go)
    {
        go.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}
