using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private PlayerManager playerManager;
    private int horizontal;
    private int vertical;

    private void Start()
    {
        playerManager = PlayerManager.Instance;
        horizontal = Animator.StringToHash("Horizontal");
        vertical = Animator.StringToHash("Vertical");
    }

    public void UpdateAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        //Animation Snapping
        float snappedHorizontal;
        float snappedVertical;

        #region Snapped Horizontal

        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            snappedHorizontal = 0.5f;
        else if (horizontalMovement > 0.55f)
            snappedHorizontal = 1f;
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            snappedHorizontal = -0.5f;
        else if (horizontalMovement < -0.55f)
            snappedHorizontal = -1f;
        else
            snappedHorizontal = 0;

        #endregion

        #region Snapped Vertical

        if (verticalMovement > 0 && verticalMovement < 0.55f)
            snappedVertical = 0.5f;
        else if (verticalMovement > 0.55f)
            snappedVertical = 1f;
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
            snappedVertical = -0.5f;
        else if (verticalMovement < -0.55f)
            snappedVertical = -1f;
        else
            snappedVertical = 0;

        #endregion

        if (isSprinting)
        {
            snappedVertical = 2f;
            snappedHorizontal = horizontalMovement;
        }

        playerManager.animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        playerManager.animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    public void PlayTargetAnimation(string targetAnimation, bool isInteracting, bool useRootMotion = false)
    {
        playerManager.animator.SetBool("isInteracting", isInteracting);
        playerManager.animator.SetBool("isUsingRootMotion", useRootMotion);
        playerManager.animator.CrossFade(targetAnimation, 0.2f);
    }

    private void OnAnimatorMove()
    {
        if (playerManager.isUsingRootMotion)
        {
            playerManager.playerRigidbody.drag = 0;
            Vector3 deltaPosition = playerManager.animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / Time.deltaTime;
            playerManager.playerRigidbody.velocity = velocity;
        }
    }
}