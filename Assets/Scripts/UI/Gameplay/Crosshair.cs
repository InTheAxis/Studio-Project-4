using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("General")]
    [SerializeField]
    [Tooltip("The object that holds all the crosshair quarters")]
    private Transform quarterHolder = null;
    [SerializeField]
    [Tooltip("The distance between each quarter from the center (in x and y)")]
    private float minRadius = 12.0f;

    [Header("Interpolation")]
    [SerializeField]
    [Tooltip("The default speed at which interpolation happens")]
    private float defaultLerpSpeed = 2.0f;
    [SerializeField]
    [Tooltip("The speed at which the crosshair interpolates when pulling an object towards you")]
    private float pullLerpSpeed = 4.0f;
    [SerializeField]
    [Tooltip("The speed at which the crosshair interpolates when throwing an object")]
    private float throwLerpSpeed = 10.0f;


    private float currentRadius= 0.0f;
    private float targetRadius = 0.0f;
    private float lerpSpeed = 2.0f;

    private bool rotating = false;
    private DestructibleController controller = null;

    private void Start()
    {
        if (NetworkOwnership.instance == null)
        {
            Debug.LogError("Offline mode interactions might not work with Crosshair");
            controller = FindObjectOfType<DestructibleController>();
        }
        else
        {
            /* Hooks onto the current player's destructible controller */
            controller = NetworkOwnership.instance.GetComponent<DestructibleController>();
        }


        /* Notify events */
        controller.pullStatus += onPullStatus;
        controller.throwStatus += onThrow;

        if(quarterHolder == null)
            quarterHolder = transform.Find("Quarters");

        currentRadius = minRadius;
        targetRadius = currentRadius;

        for (int i = 0; i < quarterHolder.childCount; ++i)
        {
            Transform t = quarterHolder.GetChild(i);
            setQuarterPosition(i, currentRadius);
        }

    }

    private void Update()
    {
        currentRadius = Mathf.Lerp(currentRadius, targetRadius, Time.deltaTime * lerpSpeed);
        for (int i = 0; i < quarterHolder.childCount; ++i)
            setQuarterPosition(i, currentRadius);

        if(rotating)
        {
            Vector3 rot = quarterHolder.localRotation.eulerAngles;
            rot.z -= 5.0f;
            quarterHolder.localRotation = Quaternion.Lerp(quarterHolder.localRotation, Quaternion.Euler(rot), Time.deltaTime * 10.0f);
        }
    }

    private void setTargetRadius(float radius)
    {
        targetRadius = radius;
    }

    private void setQuarterPosition(int i, float radius)
    {
        RectTransform t = quarterHolder.GetChild(i).GetComponent<RectTransform>();
        Vector2 anchoredPos = t.anchoredPosition;

        if (i == 0)
        {
            anchoredPos.x = -radius;
            anchoredPos.y = radius;
        }
        else if (i == 1)
        {
            anchoredPos.x = radius;
            anchoredPos.y = radius;
        }
        else if (i == 2)
        {
            anchoredPos.x = radius;
            anchoredPos.y = -radius;
        }
        else if (i == 3)
        {
            anchoredPos.x = -radius;
            anchoredPos.y = -radius;
        }

        t.anchoredPosition = anchoredPos;
    }

    private void toggleRotation(bool status)
    {
        rotating = status;
    }

    private void onPullStatus(bool status)
    {
        if (status)
        {
            targetRadius = 30.0f;
            lerpSpeed = pullLerpSpeed;
        }
        else
        {
            targetRadius = 12.0f;
            lerpSpeed = defaultLerpSpeed;

        }

        toggleRotation(status);
        rotating = status;
    }

    private void onThrow()
    {
        targetRadius = 12.0f;
        lerpSpeed = throwLerpSpeed;
        rotating = false;
    }
}
