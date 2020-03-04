using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffects : MonoBehaviour
{
    public static HitEffects instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public GameObject playerBleed = null;
    public GameObject monsterCollide = null;
    public GameObject survivorCollide = null;
    public GameObject towerExplosion = null;

}
