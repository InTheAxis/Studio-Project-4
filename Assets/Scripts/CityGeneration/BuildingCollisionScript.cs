using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCollisionScript : MonoBehaviour
{
    private void OnValidate()
    {
        Collider[] colls = Physics.OverlapSphere(new Vector3(transform.position.x, 0, transform.position.z), 15);
        foreach (Collider col in colls)
        {
            if (col.tag == "Road")
            {
                gameObject.SetActive(false);
                return;
            }
        }
    }
}