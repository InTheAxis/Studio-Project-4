using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class Minimap3D : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The plane on which the holograms are placed on")]
    private GameObject plane = null;
    [SerializeField]
    [Tooltip("The scale of the plane, used to enlarge/diminish holograms")]
    private Vector2 planeScaleRange = new Vector2(0.05f, 0.30f);
    [SerializeField]
    [Tooltip("Remaps the y scale of objects to this range on the holographic plane (planeMaxHeight / worldMaxHeight)")]
    private float planeMaxHeight = 15.0f;
    [SerializeField]
    [Tooltip("Remaps the y scale of objects to this range on the holographic plane (planeMaxHeight / worldMaxHeight)")]
    private float worldMaxHeight = 100.0f;
    [SerializeField]
    [Tooltip("The radius of the map that corresponds to the area mapped to the hologram at any given moment")]
    private float mapRadius = 300.0f;
    [SerializeField]
    [Tooltip("The sensitivity that controls how much the plane enlarges/diminish based on mouse scroll input")]
    private float scrollSensitivity = 0.1f;

    [Header("Materials")]
    [SerializeField]
    [Tooltip("The material applied to the hologram objects")]
    private Material hologramMat = null;
    [SerializeField]
    [Tooltip("The material applied to holographic landmarks")]
    private Material hologramLandmarkMat = null;

    [Header("Objects")]
    [SerializeField]
    private GameObject simpleHologram = null;
    [SerializeField]
    private GameObject complexHologram = null;
    [SerializeField]
    private GameObject humanMaleHologram = null;
    [SerializeField]
    private GameObject humanFemaleHologram = null;

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
    private float distFactor = 0.0f;
    private float scaleFactor = 0.0f;

    /* Stores the real world object as key and the hologram as value */
    private Dictionary<GameObject, GameObject> holograms = null;
    private Dictionary<GameObject, GameObject> inactive = null;
    private List<GameObject> cleanup = null;

    private bool isGrowing = false;

    private void Awake()
    {
        holograms = new Dictionary<GameObject, GameObject>();
        inactive = new Dictionary<GameObject, GameObject>();
        cleanup = new List<GameObject>();
        distFactor = plane.GetComponent<Collider>().bounds.extents.x / mapRadius;
        scaleFactor = planeMaxHeight / worldMaxHeight;

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
            if (building == null)
            {
                //cleanup.Add(building);
                continue;
            }

            Vector3 relativeWorldOffset = building.transform.position - transform.position;
            relativeWorldOffset.y = building.GetComponent<Collider>().bounds.extents.y;

            Vector3 relativePlaneOffset = relativeWorldOffset;
            relativePlaneOffset.x *= distFactor;
            relativePlaneOffset.y *= distFactor;
            relativePlaneOffset.z *= distFactor;
            Transform t = pair.Value.transform;
            t.localPosition = relativePlaneOffset;
        }

        /* Cleanup references to gameobjects that no longer exists */
        foreach (GameObject go in cleanup)
            holograms.Remove(go);
        cleanup.Clear();

        if (GameManager.playerObj != null)
        {
            Transform playerTransform = transform.root;
            
            /* Do not run this code if I am not the owner of this minimap */
            if (playerTransform != GameManager.playerObj.transform)
            {
                gameObject.SetActive(false);
                return;
            }

            float playerY = playerTransform.rotation.eulerAngles.y;

            /* Hologram Plane Rotation */
            Vector3 planeRot = plane.transform.localRotation.eulerAngles;
            planeRot.y = -playerY;
            plane.transform.localRotation = Quaternion.Slerp(plane.transform.localRotation, Quaternion.Euler(planeRot), Time.deltaTime * 5.0f);

            /* Compass Rotation */
            Vector3 frameRot = compassDirFrame.localRotation.eulerAngles;
            frameRot.z = playerY;
            compassDirFrame.localRotation = Quaternion.Slerp(compassDirFrame.transform.localRotation, Quaternion.Euler(frameRot), Time.deltaTime * 5.0f);

            float angle = playerY;
            if (angle < 0.0f)
                angle += 180.0f;
            tmAngle.text = ((int)angle).ToString();
        }
    }

    private void onShowHologram(GameObject go)
    {
        if (holograms.ContainsKey(go)) return;

        GameObject clone = null;

        /* Create new hologram */
        if (!inactive.ContainsKey(go))
        {

            

            if (go.transform.root.CompareTag("Human"))
            {
                Transform goRoot = go.transform.root;
                if(goRoot.name.ToLower().Contains("female"))
                    clone = Instantiate(humanFemaleHologram);
                else
                    clone = Instantiate(humanMaleHologram);
            }
            else
            {
                clone = Instantiate(complexHologram);


                clone.GetComponent<MeshFilter>().mesh = go.GetComponent<MeshFilter>().mesh;

                if (go.CompareTag("Landmark"))
                    clone.GetComponent<MeshRenderer>().material = hologramLandmarkMat;
                else
                    clone.GetComponent<MeshRenderer>().material = hologramMat;

                Debug.Log(go.name + " Y: " + go.GetComponent<MeshCollider>().bounds.size.y);
            }

            clone.name = go.name + " Hologram";
            holograms.Add(go, clone);
        }
        /* Fetch hologram from inactive pool */
        else
        {
            clone = inactive[go];
            holograms.Add(go, clone);
            inactive.Remove(go);
        }

        
        Vector3 relativeWorldOffset = go.transform.position - transform.position;
        Collider collider = go.GetComponent<Collider>();

        /* Offset the holographic clone's y such that the entire object is above the holographic plane (assumes the origin is center and thus, offset by half)*/
        relativeWorldOffset.y = collider.bounds.extents.y;

        /* Set holographic clone's offset to its true offset relative to plane */
        Vector3 relativePlaneOffset = relativeWorldOffset;
        relativePlaneOffset.x *= distFactor;
        relativePlaneOffset.y *= distFactor;
        relativePlaneOffset.z *= distFactor;

        /* Set holographic clone's scale to its true scale relative to plane */
        Vector3 relativePlaneScale = getRelativeScale(go);

        /* Update hologram's transform */
        Transform t = clone.transform;
        t.transform.SetParent(activeHolder);
        t.localPosition = relativePlaneOffset;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.zero;

        clone.SetActive(true);

        /* Animate the transition of clone by growing it to its target scale */
        clone.GetComponent<HologramAnimate>().Grow(relativePlaneScale);
    }

    private void onHideHologram(GameObject go)
    {
        if (!holograms.ContainsKey(go) || inactive.ContainsKey(go)) return;

        /* Shrink the object down to zero as it disappears */

        Debug.Log("Hide: " + go.name);

        holograms[go].GetComponent<HologramAnimate>().Shrink();

        /* Move active hologram to inactive pool */
        holograms[go].transform.SetParent(inactiveHolder);
        inactive.Add(go, holograms[go]);
        holograms.Remove(go);
    }


    /* Set holographic clone's size to its true scale relative to plane, which includes its transform scale multiplied by its collider size */
    private Vector3 getRelativeScale(GameObject go)
    {
        Collider collider = go.GetComponent<Collider>();
        //Vector3 colliderSize = collider.bounds.size;

        //float colliderYScale = collider.bounds.size.y;
        //colliderYScale = Mathf.Min(colliderYScale, worldMaxHeight);
        //float yRatio = colliderYScale / worldMaxHeight;
        Vector3 relativePlaneScale = go.transform.localScale;

        if (go.transform.root.CompareTag("Human"))
        {
            relativePlaneScale *= 2.0f;
        }
        else
        {
            relativePlaneScale *= 0.055f;
        }



        //relativePlaneScale.y = scaleFactor * transform.localScale.y;


        //float colliderYScale = collider.bounds.size.y * scaleFactor;
        //colliderYScale = Mathf.Min(colliderYScale, worldMaxHeight);

        //float scale = Mathf.Min(colliderYScale, distFactor);

        //float ratio = scale / worldMaxHeight;
        //Vector3 relativePlaneScale = collider.bounds.size * ratio;


        //relativePlaneScale *= yRatio;
        //relativePlaneScale.x *= scaleFactor;
        //relativePlaneScale.y *= scaleFactor;
        //relativePlaneScale.z *= scaleFactor;


        //float currentY = relativePlaneScale.y;
        //relativePlaneScale.y = planeYScaleRange.x + relativePlaneScale.y * (planeYScaleRange.y - planeYScaleRange.x) / Mathf.Min(100.0f, currentY);
        //low2 + (value - low1) * (high2 - low2) / (high1 - low1)

        return relativePlaneScale;
    }

    private void OnEnable()
    {
        /* Animate the transition of clone by growing it to its target scale */
        foreach (KeyValuePair<GameObject, GameObject> pair in holograms)
        {
            if (pair.Key == null) continue;

            pair.Value.SetActive(true);
            pair.Value.transform.localScale = Vector3.zero;
            pair.Value.GetComponent<HologramAnimate>().Grow(getRelativeScale(pair.Key));
        }

        Debug.Log("Holograms Count: " + holograms.Count);
    }


    public void Hide()
    {
        /* Shrink the object down to zero as it disappears */
        if (gameObject.activeSelf)
            StartCoroutine(shrinkHolograms());
    }

    private IEnumerator shrinkHolograms()
    {
        foreach (KeyValuePair<GameObject, GameObject> pair in holograms)
        {
            if (pair.Key == null) continue;
            Debug.Log("Hiding: " + pair.Key.name);
            pair.Value.GetComponent<HologramAnimate>().Shrink();
        }
        yield return new WaitForSeconds(0.20f);
        gameObject.SetActive(false);
    }


    public void Flicker(float duration)
    {
        StartCoroutine(flickerHolograms(duration));
    }

    private IEnumerator flickerHolograms(float duration)
    {
        

        /* Flicker Chance */
        hologramMat.SetFloat("Vector1_628109A8", 0.70f);
        hologramLandmarkMat.SetFloat("Vector1_628109A8", 0.70f);

        /* Alpha Clip */
        hologramMat.SetFloat("Vector1_6B98BE53", 0.50f);
        hologramLandmarkMat.SetFloat("Vector1_6B98BE53", 0.50f);

        ///* Scan Speed */
        //hologramMat.SetFloat("Vector1_444C18FD", 0.80f);
        //hologramLandmarkMat.SetFloat("Vector1_444C18FD", 0.80f);

        yield return new WaitForSeconds(duration);

        /* Flicker Chance */
        hologramMat.SetFloat("Vector1_628109A8", 0.02f);
        hologramLandmarkMat.SetFloat("Vector1_628109A8", 0.02f);

        /* Alpha Clip */
        hologramMat.SetFloat("Vector1_6B98BE53", 0.10f);
        hologramLandmarkMat.SetFloat("Vector1_6B98BE53", 0.10f);

        ///* Scan Speed */
        //hologramMat.SetFloat("Vector1_444C18FD", 0.12f);
        //hologramLandmarkMat.SetFloat("Vector1_444C18FD", 0.12f);


    }
}