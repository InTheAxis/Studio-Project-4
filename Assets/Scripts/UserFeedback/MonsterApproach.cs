using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterApproach : MonoBehaviour
{
    private GameObject monsterObj;

    [SerializeField]
    private float outerRange = 10;// distance from the player that is viewed as the lowest intensity

    [SerializeField]
    private float innerRange = 1;   // distance from the player that is viewed as the highest intensity

    [SerializeField]
    private MinMax flickerRate;

    private bool inRange;
    private bool fadeOut;
    private float offsetRange;

    private AudioController audioController;
    private bool flickering;
    private Minimap3D minimap;

    private void Start()
    {
        audioController = BGAudioController.instance;
        offsetRange = outerRange - innerRange;
        minimap = GetComponentInChildren<Minimap3D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (monsterObj == null)
        {
            // get monster obj
            // return
        }

        Vector3 monsterPos = Vector3.zero;
        float distFromMonster = Vector3.Distance(monsterPos, transform.position);
        if (distFromMonster < outerRange)
        {
            if (!inRange)
                EnterRange();
            inRange = true;
        }
        else
        {
            if (inRange)
                fadeOut = true;
            inRange = false;
        }
        if (inRange)
            InRange(distFromMonster);
        if (fadeOut)
            FadeOut();
    }

    private void FadeOut()
    {
        float currVol = audioController.GetVol();
        currVol = Mathf.Lerp(currVol, 0, 0.1f);
        audioController.SetVol(currVol);
        if (currVol < 0.01)
        {
            audioController.Stop();
            fadeOut = false;
        }
    }

    private void InRange(float currRange)
    {
        float intensity = Mathf.Clamp(1 - (currRange - innerRange) / offsetRange, 0.1f, 1); // range of 0 - 1
        audioController.SetVol(intensity, 0);
        // TO DO: add overlay second track
        // flicker
        if (!flickering)
        {
            if (minimap.isActiveAndEnabled)
                StartCoroutine(Flicker(intensity));
        }
    }

    private IEnumerator Flicker(float intensity)
    {
        flickering = true;
        float d1 = flickerRate.Get() / intensity;
        yield return new WaitForSeconds(d1);  // non flicker duration
        if (minimap.isActiveAndEnabled)
        {
            float d2 = flickerRate.Get() * intensity;
            minimap.Flicker(d2);
            yield return new WaitForSeconds(d2);
            Debug.Log("flicker: " + d2);
        }
        flickering = false;
    }

    private void EnterRange()
    {
        inRange = true;
        fadeOut = false;
        audioController.Play("jaws0");
    }

    private void OnDrawGizmos()
    {
        Vector3 monsterPos = Vector3.zero;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(monsterPos, outerRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(monsterPos, innerRange);
    }
}