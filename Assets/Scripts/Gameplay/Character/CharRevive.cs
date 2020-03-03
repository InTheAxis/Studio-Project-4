using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharRevive : MonoBehaviour
{
    [SerializeField]
    private GameObject reviveInteractable;
    [SerializeField]
    private CharHealth health;

    private void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        Debug.Log("callback attached");
        health.OnDead += SpawnPlayerRevive;
    }
    private void OnDisable()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        
        health.OnDead -= SpawnPlayerRevive;
    }

    private void SpawnPlayerRevive(int id)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject newObj = PhotonNetwork.InstantiateSceneObject(reviveInteractable.name, transform.position, transform.rotation);
            Vector3 scale = transform.localScale;
            scale.Scale(newObj.transform.localScale);
            newObj.transform.localScale = scale;
            newObj.GetComponent<InteractableRevive>().SetPlayerToRevive(id);
            Debug.Log("Revive obj spawned(Master)");
        }
    }
}
