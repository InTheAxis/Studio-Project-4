using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap3D : MonoBehaviour
{

    [SerializeField]
    private GameObject simpleCube = null;
    [SerializeField]
    private Camera minimapCam = null;
    [SerializeField]
    private string maskName = "Environment";
    [SerializeField]
    private float scrollSensitivity = 0.1f;
    [SerializeField]
    private Vector2 planeScaleRange = new Vector2(0.05f, 0.30f);

    private GameObject[] buildings = null;
    private int buildingMaskLayer = 0;
    private float planeScale = 0.25f;

    private List<GameObject> visible = null;
    private List<GameObject> holographic = null;

    private Dictionary<GameObject, GameObject> pairs = null;

    private void Start()
    {


    }

    private void Initialize()
    {
        minimapCam = CharMinimapCamera.Instance.GetComponent<Camera>();
        buildingMaskLayer = LayerMask.NameToLayer(maskName);
        buildings = getAllObjectsInLayer(buildingMaskLayer);
        visible = new List<GameObject>();
        holographic = new List<GameObject>();
        pairs = new Dictionary<GameObject, GameObject>();

    }

    public void Update()
    {
        float deltaY = Input.mouseScrollDelta.y * scrollSensitivity;
        if (deltaY != 0.0f)
        {
            planeScale = Mathf.Clamp(planeScale + deltaY, planeScaleRange.x, planeScaleRange.y);
            transform.localScale = new Vector3(planeScale, planeScale, planeScale);
        }

        for(int i = 0; i < visible.Count; ++i)
        {
            if (visible[i] == null) continue;

            Vector3 screenPoint = minimapCam.WorldToViewportPoint(visible[i].transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            if (!onScreen)
            {
                GameObject clone = pairs[visible[i]];
                Destroy(clone);

                pairs.Remove(visible[i]);
                visible.RemoveAt(i--);
            }

        }
    }

    public void Show()
    {
        if (visible == null)
            Initialize();

        //foreach(GameObject building in holographic)
        //    Destroy(building);

        //holographic.Clear();

        //Debug.Log("Visible Buildings: " + visible.Count);
        //Debug.Log("Minimap Size: " + minimapCam.orthographicSize);

        //Debug.Log("Extents X: " + GetComponent<Collider>().bounds.extents.x);
        //float rawScaleFactor = GetComponent<Collider>().bounds.extents.x / (minimapCam.orthographicSize * transform.localScale.x);
        //float scaleFactor = GetComponent<Collider>().bounds.extents.x / minimapCam.orthographicSize;
        //Debug.Log("Scale Factor: " + scaleFactor);

        float rawScaleFactor = GetComponent<Collider>().bounds.extents.x / minimapCam.orthographicSize;
        float scaleFactor = GetComponent<Collider>().bounds.extents.x / minimapCam.orthographicSize;
        Vector3 scaleFactorVec3 = new Vector3(scaleFactor, scaleFactor, scaleFactor);

        foreach (GameObject building in buildings)
        {
            if (building == null) continue;

            Vector3 screenPoint = minimapCam.WorldToViewportPoint(building.transform.position);
            bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            if (onScreen && !pairs.ContainsKey(building))
            {
                visible.Add(building);
                Vector3 relativeWorldOffset = building.transform.position - minimapCam.transform.position;
                relativeWorldOffset.y = building.GetComponent<Collider>().bounds.extents.y;
                //Debug.L
                //Debug.Log(building.name + " -> World Offset: " + relativeWorldOffset);

                Vector3 relativePlaneOffset = relativeWorldOffset;
                relativePlaneOffset.x *= rawScaleFactor;
                relativePlaneOffset.y *= rawScaleFactor;
                relativePlaneOffset.z *= rawScaleFactor;
                //Debug.Log(v.name + " -> Plane Offset: " + relativePlaneOffset);


                Vector3 relativePlaneScale = building.transform.localScale;
                relativePlaneScale.x *= rawScaleFactor;
                relativePlaneScale.y *= scaleFactor;
                relativePlaneScale.z *= rawScaleFactor;
                //Debug.Log(v.name + " -> Plane Scale: " + relativePlaneScale);

                GameObject clone = Instantiate(simpleCube);
                Transform t = clone.transform;
                t.SetParent(transform);                                         
                t.localPosition = relativePlaneOffset;
                t.localRotation = Quaternion.identity;
                t.localScale = relativePlaneScale;

                holographic.Add(clone);
                pairs.Add(building, clone);
            }

        }

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
