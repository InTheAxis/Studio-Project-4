using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharMinimapCamera : MonoBehaviour
{
    public static CharMinimapCamera Instance;

    [SerializeField]
    private float minimapY = 50.0f;
    [SerializeField]
    private GameObject minimapCanvas = null;

    private CharTPController charControl = null;

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        if (charControl != null)
        {
            Vector3 position = charControl.transform.position;
            position.y = minimapY;
            transform.position = position;

            Vector3 rotation = charControl.transform.rotation.eulerAngles;
            rotation.x = 90.0f;
            rotation.y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }

    public void SetCharController(CharTPController cc)
    {
        charControl = cc;
        minimapCanvas = charControl.transform.Find("Canvas").gameObject;
    }

    public GameObject getMinimap()
    {
        return minimapCanvas;
    }
}
