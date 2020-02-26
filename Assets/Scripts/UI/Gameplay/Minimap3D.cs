using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

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
    private GameObject holographicCentralTower = null;

    [SerializeField]
    private float scrollSensitivity = 0.1f;

    [SerializeField]
    private Material hologramMat = null;

    [SerializeField]
    private string controlTowerTag = "Landmark";

    [Header("Storage")]
    [SerializeField]
    private Transform activeHolder = null;

    [SerializeField]
    private Transform inactiveHolder = null;

    [Header("UI")]
    [SerializeField]
    private Transform compassDirFrame = null;

    [SerializeField]
    private TextMeshPro tmAngle = null;

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
        Debug.Log(plane.GetComponent<Collider>().bounds.size.x);
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
        //Debug.Log("Plane Pos: " + plane.transform.position);
        foreach (KeyValuePair<GameObject, GameObject> pair in holograms)
        {
            GameObject building = pair.Key;
            if (building == null)
            {
                Debug.LogError("Building is null");
                continue;
            }
            Vector3 relativeWorldOffset = building.transform.position - transform.position;
            relativeWorldOffset.y = building.GetComponent<Collider>().bounds.extents.y;

            Vector3 relativePlaneOffset = relativeWorldOffset;
            relativePlaneOffset.x *= scaleFactor;
            relativePlaneOffset.y *= scaleFactor;
            relativePlaneOffset.z *= scaleFactor;

            //Vector3 relativePlaneScale = building.transform.localScale;
            //relativePlaneScale.x *= scaleFactor;
            //relativePlaneScale.y *= scaleFactor;
            //relativePlaneScale.z *= scaleFactor;

            /* Update hologram's transform */
            Transform t = pair.Value.transform;
            t.localPosition = relativePlaneOffset;
            //t.localScale = relativePlaneScale;
            //Debug.Log(relativePlaneOffset.magnitude);
        }

        if (GameManager.playerObj != null)
        {
            Transform playerTransform = transform.root;
            if (playerTransform != GameManager.playerObj.transform)
            {
                gameObject.SetActive(false);
                return;
            }

            float playerY = playerTransform.rotation.eulerAngles.y;

            Vector3 planeRot = plane.transform.localRotation.eulerAngles;
            planeRot.y = -playerY;
            plane.transform.localRotation = Quaternion.Slerp(plane.transform.localRotation, Quaternion.Euler(planeRot), Time.deltaTime * 5.0f);

            Vector3 frameRot = compassDirFrame.localRotation.eulerAngles;
            frameRot.z = playerY;
            compassDirFrame.localRotation = Quaternion.Slerp(compassDirFrame.transform.localRotation, Quaternion.Euler(frameRot), Time.deltaTime * 5.0f);

            float angle = playerY;
            if (angle < 0.0f)
                angle += 180.0f;
            tmAngle.text = ((int)angle).ToString();
            //tmAngle =
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

    private void onShowHologram(GameObject go)
    {
        if (holograms.ContainsKey(go)) return;

        GameObject clone = null;

        /* Create new hologram */
        if (!inactive.ContainsKey(go))
        {
            if (go.CompareTag("Landmark"))
                clone = Instantiate(holographicCentralTower);
            else
                clone = Instantiate(simpleCube);
            clone.name = go.name + " Hologram";
            holograms.Add(go, clone);
            //clone.GetComponent<HologramAnimate>().Grow(new Vector3(1.0f, 1.0f, 1.0f));
        }
        /* Fetch hologram from inactive pool */
        else
        {
            clone = inactive[go];
            //clone.GetComponent<HologramAnimate>().Grow(new Vector3(1.0f, 1.0f, 1.0f));
            //clone.transform.SetParent(activeHolder);
            //clone.transform.localRotation = Quaternion.identity;
            holograms.Add(go, clone);
            inactive.Remove(go);
        }

        Vector3 relativeWorldOffset = go.transform.position - transform.position;
        relativeWorldOffset.y = go.GetComponent<Collider>().bounds.extents.y;

        Vector3 relativePlaneOffset = relativeWorldOffset;
        relativePlaneOffset.x *= scaleFactor;
        relativePlaneOffset.y *= scaleFactor;
        relativePlaneOffset.z *= scaleFactor;

        Vector3 relativePlaneScale = go.transform.localScale;
        relativePlaneScale.x *= scaleFactor;
        relativePlaneScale.y *= scaleFactor;
        relativePlaneScale.z *= scaleFactor;

        /* Update hologram's transform */
        Transform t = clone.transform;
        t.transform.SetParent(activeHolder);
        t.localPosition = relativePlaneOffset;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.zero;

        clone.SetActive(true);
        clone.GetComponent<HologramAnimate>().Grow(relativePlaneScale);
    }

    private void onHideHologram(GameObject go)
    {
        if (!holograms.ContainsKey(go) || inactive.ContainsKey(go)) return;

        /* Move active hologram to inactive pool */
        holograms[go].GetComponent<HologramAnimate>().Shrink();
        holograms[go].transform.SetParent(inactiveHolder);
        inactive.Add(go, holograms[go]);
        holograms.Remove(go);
    }

    private Vector3 getRelativeScale(GameObject go)
    {
        Vector3 relativePlaneScale = go.transform.localScale;
        relativePlaneScale.x *= scaleFactor;
        relativePlaneScale.y *= scaleFactor;
        relativePlaneScale.z *= scaleFactor;

        if (go.CompareTag(controlTowerTag))
        {
            Debug.Log(relativePlaneScale);
            Debug.Log(go.GetComponent<Collider>().bounds.size.y);
        }

        return relativePlaneScale;
    }

    private void OnEnable()
    {
        foreach (KeyValuePair<GameObject, GameObject> pair in holograms)
        {
            pair.Value.transform.localScale = Vector3.zero;
            pair.Value.GetComponent<HologramAnimate>().Grow(getRelativeScale(pair.Key));
        }
    }

    public void Hide()
    {
        if (gameObject.activeSelf)
            StartCoroutine(shrinkHolograms());
    }

    private IEnumerator shrinkHolograms()
    {
        foreach (KeyValuePair<GameObject, GameObject> pair in holograms)
        {
            pair.Value.GetComponent<HologramAnimate>().Shrink();
        }
        yield return new WaitForSeconds(0.20f);
        gameObject.SetActive(false);
    }
}