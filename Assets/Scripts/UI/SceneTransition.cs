using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTransition : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Prefab that holds the transition animator.")]
    private GameObject transition = null;


    private AsyncOperation loadAsync = null;

    public void Transition(string targetScene, string preTransition,
        string postTransition, float multiplier = 1.0f)
    {
        StartCoroutine(transitionScene(targetScene, preTransition, postTransition, multiplier));
    }

    private IEnumerator transitionScene(string targetScene, string preTransition,
        string postTransition, float multiplier)
    {
        GameObject t = Instantiate(transition);
        Animator animator = t.GetComponent<Animator>();
        animator.SetFloat("runMultiplier", multiplier);
        animator.Play(preTransition);

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length / multiplier);
        StartCoroutine(loadLevel(targetScene, postTransition, multiplier));
    }

    private IEnumerator loadLevel(string targetScene, string postTransition, float multiplier)
    {
        loadAsync = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
        while (!loadAsync.isDone)
        {
            yield return null;
        }
        GameObject t = Instantiate(transition);
        Animator animator = t.GetComponent<Animator>();
        animator.SetFloat("runMultiplier", multiplier);
        animator.Play(postTransition);
        Destroy(t, animator.GetCurrentAnimatorStateInfo(0).length / multiplier);

    }


}
