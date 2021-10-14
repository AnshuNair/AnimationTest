using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBool : StateMachineBehaviour
{
    public string isInteracting;
    public bool isInteractingBool;

    public string isUsingRootMotion;
    public bool isUsingRootMotionBool;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isInteracting != "")
            animator.SetBool(isInteracting, isInteractingBool);
        if (isUsingRootMotion != "")
            animator.SetBool(isUsingRootMotion, isUsingRootMotionBool);
    }
}
