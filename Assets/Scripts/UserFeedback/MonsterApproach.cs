using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MonsterApproach : MonoBehaviour
{
    private GameObject monsterObj = null;

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
    private bool layer0;
    private bool layer1;
    private bool layer2;

    private void Awake()
    {
        audioController = BGAudioController.instance;
        offsetRange = outerRange - innerRange;
        minimap = GetComponentInChildren<Minimap3D>();
        audioController.SetMusic();
        if (PhotonNetwork.IsMasterClient)
            enabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!minimap)
            minimap = GetComponentInChildren<Minimap3D>();
        if (monsterObj == null)
        {
            if (!GameManager.monsterObj)
                return;
            monsterObj = GameManager.monsterObj;
        }

        Vector3 monsterPos = monsterObj.transform.position;
        //Vector3 monsterPos = Vector3.zero;
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
        audioController.SetVol(currVol, 1);
        audioController.SetVol(currVol, 2);
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
        // add overlay second track
        if (intensity > 0.4 && !layer1)
        {
            Debug.Log("Layer1");
            layer1 = true;
            audioController.Play("approach1", 1);
        }
        if (intensity < 0.4)
        {
            layer1 = false;
            layer2 = false;
            audioController.Stop(1);
            audioController.Stop(2);
        }
        if (intensity < 0.8)
        {
            layer2 = false;
            audioController.Stop(2);
        }
        // add overlay third track
        if (intensity > 0.8 && !layer2)
        {
            Debug.Log("Layer2");
            layer2 = true;
            audioController.Play("approach2", 2);
        }
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
        layer0 = true;
        audioController.Play("approach0");
    }

    private void OnDrawGizmos()
    {
        if (monsterObj == null)
        {
            if (!GameManager.monsterObj)
                return;
            monsterObj = GameManager.monsterObj;
        }

        Vector3 monsterPos = monsterObj.transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(monsterPos, outerRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(monsterPos, innerRange);
    }
}