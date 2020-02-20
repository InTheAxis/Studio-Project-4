using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMaskEffect : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField]
    private Vector3 startPos = Vector3.zero;
    [SerializeField]
    private Vector3 endPos = Vector3.zero;

    [SerializeField]
    private float moveSpeed = 18.0f;

    private RectTransform rt = null;
    private bool isAnimating = false;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(isAnimating)
        {
            Vector3 dist = endPos - rt.anchoredPosition3D;
            if(dist.sqrMagnitude <= 0.25f)
            {
                isAnimating = false;
            }
            else
            {
                Vector3 current = rt.anchoredPosition3D;
                current -= transform.right * moveSpeed;
                rt.anchoredPosition3D = current;
            }   
        }
    }

    public void Begin(float startY)
    {
        Vector3 anchoredPos = startPos;
        anchoredPos.y = startY;
        rt.anchoredPosition3D = anchoredPos;
        isAnimating = true;
    }
}
