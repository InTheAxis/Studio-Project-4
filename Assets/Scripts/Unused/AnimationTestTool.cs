#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

[RequireComponent(typeof(Animator))]
public class AnimationTestTool : MonoBehaviour
{
    public KeyCode key = KeyCode.Space;

    public AnimationClip animationClip;

    public string animationStateName = "MyAnimationState";

    public string animationControllerName = "myAnimController";

    public string filePath = "Assets/";



    private Animator animator;
    private string completeFilePath;
    private const string triggerName = "keypress";

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (!animator)
            Debug.LogError("Animator not found, add one or contact gdt lads");

        completeFilePath = filePath + animationControllerName + ".controller";
    }

    private void Start()
    {
        if (animator.runtimeAnimatorController == null)
            animator.runtimeAnimatorController = CreateController();
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            animator.SetTrigger(triggerName);
            Debug.Log("Animation played");
        }
    }

    private AnimatorController CreateController()
    {
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(completeFilePath);
        if (!controller)
            Debug.LogError("Couldn't create animation controller at " + completeFilePath);

        controller.AddParameter(triggerName, AnimatorControllerParameterType.Trigger);
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
        AnimatorState state = rootStateMachine.AddState(animationStateName);
        state.motion = animationClip;

        AnimatorStateTransition transition =  rootStateMachine.AddAnyStateTransition(state);
        transition.AddCondition(AnimatorConditionMode.If, 0, triggerName);

        return controller;
    }
}

#endif