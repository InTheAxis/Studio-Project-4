using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerWeightChange : StateMachineBehaviour
{
    [SerializeField]
    private float initialWeight;
    [SerializeField]
    private float targetWeight;
    [SerializeField]
    private float lerpDuration;

    private float weight;
    private float timer;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
        weight = initialWeight;
        animator.SetLayerWeight(layerIndex, initialWeight);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (targetWeight != weight && lerpDuration > 0)
        {
            timer += Time.deltaTime;
            weight = Mathf.Lerp(initialWeight, targetWeight, timer / lerpDuration);
            if (timer >= lerpDuration)
                weight = targetWeight;
            animator.SetLayerWeight(layerIndex, weight);
        }
    }
}
