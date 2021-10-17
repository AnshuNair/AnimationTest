using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    public static PlayerLocomotion Instance { get; private set; }

    private PlayerManager playerManager;
    private Vector3 moveDirection;
    private Transform cameraObject;

    [HideInInspector] public float inAirTimer;
    [HideInInspector] public float leapingVelocity = 3f;
    [HideInInspector] public float fallingVelocity = 33f;
    [HideInInspector] public float rayCastHeightOffset = 0.5f;
    [Header("Falling")]
    public LayerMask groundLayer;

    [Header("Movement Flags")]
    [HideInInspector] public bool isSprinting;
    [HideInInspector] public bool startedSprinting;
    [HideInInspector] public bool isWalking;
    [HideInInspector] public bool isRolling;
    [HideInInspector] public bool isGrounded = true;
    [HideInInspector] public float currentSpeed;

    private float walkingSpeed = 2.5f;
    private float runningSpeed = 5f;
    private float sprintingSpeed = 7f;
    private float rotationSpeed = 15f;


    [HideInInspector] public float jumpHeight = 1f;
    [HideInInspector] public float gravityIntensity = -10f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        playerManager = PlayerManager.Instance;
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();

        if (playerManager.isInteracting)
            return;

        HandleMovement();
        HandleRotation();
        HandleRollingAndSprinting();
    }

    private void HandleMovement()
    {
        if (isRolling)
            return;

        moveDirection = cameraObject.forward * playerManager.inputManager.verticalInput;
        moveDirection += cameraObject.right * playerManager.inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (startedSprinting)
        {
            if (currentSpeed < sprintingSpeed)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, sprintingSpeed, Time.deltaTime * 2f);
                moveDirection *= currentSpeed;
            }
        }
        else
        {
            if (isSprinting)
            {
                moveDirection *= sprintingSpeed;
            }
            else
            {
                if (!isWalking)
                {
                    moveDirection *= runningSpeed;
                }
                else
                {
                    moveDirection *= walkingSpeed;
                }
            }
        }

        Vector3 movementVelocity = moveDirection;
        playerManager.playerRigidbody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;
        targetDirection = cameraObject.forward * playerManager.inputManager.verticalInput;
        targetDirection += cameraObject.right * playerManager.inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += rayCastHeightOffset;
        Vector3 targetPosition = transform.position;

        if (!isGrounded)
        {
            if (!playerManager.isInteracting)
            {
                playerManager.animationManager.PlayTargetAnimation("Falling", true);
            }

            playerManager.animator.SetBool("isUsingRootMotion", false);
            inAirTimer += Time.deltaTime;
            playerManager.playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerManager.playerRigidbody.AddForce(Vector3.down * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, groundLayer))
        {
            if (!isGrounded)
            {
                playerManager.animationManager.PlayTargetAnimation("Land", true);
            }

            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (isGrounded)
        {
            if (playerManager.isInteracting || playerManager.inputManager.moveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    private void HandleRollingAndSprinting()
    {
        if (playerManager.isInteracting)
            return;


        if (isRolling)
        {
            moveDirection = cameraObject.forward * playerManager.inputManager.verticalInput;
            moveDirection += cameraObject.right * playerManager.inputManager.horizontalInput;

            if (playerManager.inputManager.moveAmount > 0)
            {
                playerManager.animationManager.PlayTargetAnimation("Rolling", true, true);
                moveDirection.y = 0;
                Quaternion rollRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = rollRotation;
                isRolling = false;
            }
            else
            {
                playerManager.animationManager.PlayTargetAnimation("Backstep", true, true);
                isRolling = false;
            }
        }
    }

    public float GetSpeedFromAnimatorParameter()
    {
        float mySpeed = 0f;
        float vertical = playerManager.animator.GetFloat("Vertical");

        if (vertical == 0.5f)
        {
            mySpeed = 2.5f;
        }
        else if (vertical == 1f)
        {
            mySpeed = 5f;
        }
        else if (vertical == 2f)
        {
            mySpeed = 7f;
        }

        return mySpeed;
    }
}
