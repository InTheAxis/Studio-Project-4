using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Revive : MonoBehaviour
{
    [SerializeField]
    private GameObject reviveInteractable;

    private void OnEnable()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        foreach (var p in CharTPController.PlayerControllerRefs)
            p.controller.GetComponent<CharHealth>().OnDead += SpawnPlayerRevive;
    }
    private void OnDisable()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        foreach (var p in CharTPController.PlayerControllerRefs)
            p.controller.GetComponent<CharHealth>().OnDead -= SpawnPlayerRevive;
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
            Debug.Log("Revive obj spawned");
        }
    }
}
