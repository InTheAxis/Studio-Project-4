using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField]
    private RawImage compassScale = null;
    [SerializeField]
    private Transform playerTransform = null;
    [SerializeField]
    private RectTransform scroll = null;



    private int firstVisibleIndex = 0;
    private float distBetweenIndicator = 0;
    private float totalScrollDist = 0;

    private void Start()
    {
        if (playerTransform == null)
            playerTransform = FindObjectOfType<CharTPController>().transform;

        distBetweenIndicator = scroll.GetChild(1).GetComponent<RectTransform>().anchoredPosition.x - scroll.GetChild(0).GetComponent<RectTransform>().anchoredPosition.x;
        totalScrollDist = distBetweenIndicator * 8.0f;

        for(int i = 0; i < scroll.childCount; ++i)
        {
            RectTransform t = scroll.GetChild(i).GetComponent<RectTransform>();
            Vector2 anchoredPos = t.anchoredPosition;
            anchoredPos.x = (i - 4) * distBetweenIndicator;
            t.anchoredPosition = anchoredPos;
        }


    }

    private void Update()
    {
     
        float angle = Vector3.SignedAngle(Vector3.forward, playerTransform.forward, Vector3.up);
        Rect rect = compassScale.uvRect;
        rect.x = (angle / 15.0f) * 0.167f;
        compassScale.uvRect = rect;

        Vector2 anchoredPos = scroll.anchoredPosition;
        float angleFactor = angle / 45.0f;
        anchoredPos.x = -angleFactor * distBetweenIndicator;
        scroll.anchoredPosition = anchoredPos;
    }
}
