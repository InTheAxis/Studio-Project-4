using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap3D : MonoBehaviour
{

    [SerializeField]
    private GameObject plane = null;
    [SerializeField]
    private Vector2 planeScaleRange = new Vector2(0.05f, 0.30f);
    [SerializeField]
    private float mapRadius = 300.0f;
    [SerializeField]
    private GameObject simpleCube = null;
    [SerializeField]
    private float scrollSensitivity = 0.1f;

    [Header("Storage")]
    [SerializeField]
    private Transform activeHolder = null;
    [SerializeField]
    private Transform inactiveHolder = null;

    [Header("UI")]
    [SerializeField]
    private Transform displayHolder = null;
    [SerializeField]
    private Transform compassDirFrame = null;
    [SerializeField]
    private Transform compassDottedFrame = null;

    private float planeScale = 0.25f;
    private float scaleFactor = 0.0f;

    /* Stores the real world object as key and the hologram as value */
    private Dictionary<GameObject, GameObject> holograms = null;
    private Dictionary<GameObject, GameObject> inactive = null;


    private void Awake()
    {
        holograms = new Dictionary<GameObject, GameObject>();
        inactive = new Dictionary<GameObject, GameObject>();
        scaleFactor = plane.GetComponent<Collider>().bounds.size.x / mapRadius;
        MinimapTrigger trigger = GetComponentInChildren<MinimapTrigger>();
        trigger.setBounds(mapRadius);
        trigger.showHologram += onShowHologram;
        trigger.hideHologram += onHideHologram;


    }

    public void Update()
    {

        /* Zoom in/out */
        float deltaY = Input.mouseScrollDelta.y * scrollSensitivity;
        if (deltaY != 0.0f)
        {
            planeScale = Mathf.Clamp(planeScale + deltaY, planeScaleRange.x, planeScaleRange.y);
            plane.transform.localScale = new Vector3(planeScale, planeScale, planeScale);
        }

        /* Update hologram's transform */
        foreach (KeyValuePair<GameObject, GameObject> pair in holograms)
        {
            GameObject building = pair.Key;
            Vector3 relativeWorldOffset = building.transform.position - transform.position;
            relativeWorldOffset.y = building.GetComponent<Collider>().bounds.extents.y;

            Vector3 relativePlaneOffset = relativeWorldOffset;
            relativePlaneOffset.x *= scaleFactor;
            relativePlaneOffset.y *= scaleFactor;
            relativePlaneOffset.z *= scaleFactor;

            Vector3 relativePlaneScale = building.transform.localScale;
            relativePlaneScale.x *= scaleFactor;
            relativePlaneScale.y *= scaleFactor;
            relativePlaneScale.z *= scaleFactor;

            /* Update hologram's transform */
            Transform t = pair.Value.transform;
            t.localPosition = relativePlaneOffset;
            t.localScale = relativePlaneScale;
        }


        if(GameManager.playerObj != null)
        {
            Transform playerTransform = GameManager.playerObj.transform;
            float playerY = playerTransform.rotation.eulerAngles.y;
            transform.position = playerTransform.position;
            transform.rotation = playerTransform.rotation;

            Vector3 planeRot = plane.transform.localRotation.eulerAngles;
            planeRot.y = playerY;
            plane.transform.localRotation = Quaternion.Slerp(plane.transform.localRotation, Quaternion.Euler(planeRot), Time.deltaTime * 5.0f);

            Vector3 frameRot = compassDirFrame.localRotation.eulerAngles;
            frameRot.y = playerY;
            compassDirFrame.localRotation = Quaternion.Slerp(plane.transform.localRotation, Quaternion.Euler(frameRot), Time.deltaTime * 5.0f);

            //Vector3 dottedPos = compassDottedFrame.transform.localPosition;
            //dottedPos.y = -0.05f + 2.0f * Mathf.Sin(Time.deltaTime * 10.0f);
            //compassDottedFrame.transform.localPosition = dottedPos;
        }
        //for(int i = 0; i < visible.Count; ++i)
        //{
        //    if (visible[i] == null) continue;

        //    Vector3 screenPoint = minimapCam.WorldToViewportPoint(visible[i].transform.position);
        //    bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        //    if (!onScreen)
        //    {
        //        GameObject clone = pairs[visible[i]];
        //        Destroy(clone);

        //        pairs.Remove(visible[i]);
        //        visible.RemoveAt(i--);
        //    }

        //}
    }

    public void Show()
    {
        //if (visible == null)
        //    Initialize();

        //foreach(GameObject building in holographic)
        //    Destroy(building);

        //holographic.Clear();

        //Debug.Log("Visible Buildings: " + visible.Count);
        //Debug.Log("Minimap Size: " + minimapCam.orthographicSize);

        //Debug.Log("Extents X: " + GetComponent<Collider>().bounds.extents.x);
        //float rawScaleFactor = GetComponent<Collider>().bounds.extents.x / (minimapCam.orthographicSize * transform.localScale.x);
        //float scaleFactor = GetComponent<Collider>().bounds.extents.x / minimapCam.orthographicSize;
        //Debug.Log("Scale Factor: " + scaleFactor);

        //float rawScaleFactor = GetComponent<Collider>().bounds.extents.x / minimapCam.orthographicSize;
        //float scaleFactor = GetComponent<Collider>().bounds.extents.x / minimapCam.orthographicSize;
        //Vector3 scaleFactorVec3 = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        //foreach (GameObject building in buildings)
        //{
        //    if (building == null) continue;

        //    Vector3 screenPoint = minimapCam.WorldToViewportPoint(building.transform.position);
        //    bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        //    if (onScreen && !pairs.ContainsKey(building))
        //    {
        //        visible.Add(building);
        //        Vector3 relativeWorldOffset = building.transform.position - minimapCam.transform.position;
        //        relativeWorldOffset.y = building.GetComponent<Collider>().bounds.extents.y;
        //        //Debug.L
        //        //Debug.Log(building.name + " -> World Offset: " + relativeWorldOffset);

        //        Vector3 relativePlaneOffset = relativeWorldOffset;
        //        relativePlaneOffset.x *= rawScaleFactor;
        //        relativePlaneOffset.y *= rawScaleFactor;
        //        relativePlaneOffset.z *= rawScaleFactor;
        //        //Debug.Log(v.name + " -> Plane Offset: " + relativePlaneOffset);


        //        Vector3 relativePlaneScale = building.transform.localScale;
        //        relativePlaneScale.x *= rawScaleFactor;
        //        relativePlaneScale.y *= scaleFactor;
        //        relativePlaneScale.z *= rawScaleFactor;
        //        //Debug.Log(v.name + " -> Plane Scale: " + relativePlaneScale);

        //        GameObject clone = Instantiate(simpleCube);
        //        Transform t = clone.transform;
        //        t.SetParent(transform);                                         
        //        t.localPosition = relativePlaneOffset;
        //        t.localRotation = Quaternion.identity;
        //        t.localScale = relativePlaneScale;

        //        holographic.Add(clone);
        //        pairs.Add(building, clone);
        //    }

        //}

    }


    private void onShowHologram(GameObject go)
    {
        if (holograms.ContainsKey(go)) return;

        /* Create new hologram */
        if (!inactive.ContainsKey(go))
        {
            GameObject clone = Instantiate(simpleCube);
            clone.name = go.name + " Hologram";
            holograms.Add(go, clone);
            clone.transform.SetParent(activeHolder);
            clone.transform.localRotation = Quaternion.identity;
        }
        /* Fetch hologram from inactive pool */
        else
        {
            GameObject existingClone = inactive[go];
            existingClone.SetActive(true);
            existingClone.transform.SetParent(activeHolder);
            existingClone.transform.localRotation = Quaternion.identity;
            holograms.Add(go, existingClone);
            inactive.Remove(go);

        }

    }

    private void onHideHologram(GameObject go)
    {
        if (!holograms.ContainsKey(go) || inactive.ContainsKey(go)) return;

        /* Move active hologram to inactive pool */
        holograms[go].SetActive(false);
        holograms[go].transform.SetParent(inactiveHolder);
        inactive.Add(go, holograms[go]);
        holograms.Remove(go);

    }

    private GameObject[] getAllObjectsInLayer(int layer)
    {
        GameObject[] gos = FindObjectsOfType<GameObject>();
        List<GameObject> goList = new List<GameObject>();

        foreach (GameObject go in gos)
        {
            if (go.layer == layer && go.name != "Ground")
            {
                goList.Add(go);
            }
        }

        if (goList.Count == 0)
            return null;
        else
            return goList.ToArray();
    }



}
