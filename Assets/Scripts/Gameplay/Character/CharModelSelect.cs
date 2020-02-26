using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharModelSelect : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    List<GameObject> modelOptions;
    [SerializeField]
    List<Avatar> correspondingAvatars;
    [SerializeField]
    List<Avatar> correspondingControllers;

    private bool selected = false;

    private void Start()
    {
        if (!selected)
            SelectModel(0);
    }

    public void SelectModel(int idx)
    {
        if (idx < 0 || idx >= modelOptions.Count)
        {
            Debug.LogErrorFormat("No such model of idx {0}", idx);
            return;
        }

        animator.avatar = correspondingAvatars[idx];
        DisableAllModels();
        modelOptions[idx].SetActive(true);
        selected = true;

        Debug.LogFormat("Model {0} Selected", idx);
    }

    private void DisableAllModels()
    {
        foreach(GameObject go in modelOptions)
            go.SetActive(false);
    }
}
