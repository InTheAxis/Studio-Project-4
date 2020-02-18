using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//acts like a tag and store damage dealt. 
//put this on things that can hit stuff, on same level as collider.

[RequireComponent(typeof(Collider))]
public class DamageData : MonoBehaviour
{
    public int dmg = 1;
}
