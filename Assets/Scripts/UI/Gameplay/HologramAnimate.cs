using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramAnimate : MonoBehaviour
{
    [SerializeField]
    private float speed = 5.0f;

    private Vector3 targetScale = Vector3.zero;

    private bool grow = false;
    private bool shrink = false;


    private void Update()
    {
        Vector3 currentScale = transform.localScale;

        if (grow || shrink)
        {
            currentScale = Vector3.Lerp(currentScale, targetScale, speed * Time.deltaTime);
            transform.localScale = currentScale;
            if ((targetScale - currentScale).sqrMagnitude <= 0.02f)
            {
                if (shrink)
                    gameObject.SetActive(false);
                grow = shrink = false;
            }
        }
    }

    public void Grow(Vector3 targetScale)
    {

        this.targetScale = targetScale;
        grow = true;
        shrink = false;
    }

    public void Shrink()
    {
        targetScale = Vector3.zero;
        grow = false;
        shrink = true;
    }

    
}
