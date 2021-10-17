using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBool : StateMachineBehaviour
{
    public string isInteracting;
    public bool isInteractingBool;

    public string isUsingRootMotion;
    public bool isUsingRootMotionBool;

    public bool startedSprinting;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isInteracting != "")
            animator.SetBool(isInteracting, isInteractingBool);
        if (isUsingRootMotion != "")
            animator.SetBool(isUsingRootMotion, isUsingRootMotionBool);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (startedSprinting && stateInfo.IsName("Idle To Sprint"))
        {
            PlayerManager.Instance.playerLocomotion.isSprinting = true;
            PlayerManager.Instance.playerLocomotion.startedSprinting = false;
        }
    }
}
