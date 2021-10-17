using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerControls playerControls;
    private PlayerManager playerManager;

    [HideInInspector] public Vector2 movementInput;
    [HideInInspector] public Vector2 cameraInput;
    [HideInInspector] public float cameraInputX;
    [HideInInspector] public float cameraInputY;
    [HideInInspector] public float moveAmount;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float rollInputTimer;
    [HideInInspector] public bool sprintInput;
    [HideInInspector] public bool walkInput;

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

        playerControls = new PlayerControls();
        playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        playerControls.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

        playerControls.Enable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleRollInput();
        HandleWalkInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        playerManager.animationManager.UpdateAnimatorValues(0, moveAmount);

    }

    private void HandleRollInput()
    {
        sprintInput = playerControls.PlayerMovement.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Started;

        if (sprintInput && !playerManager.isInteracting)
        {
            rollInputTimer += Time.deltaTime;
            if (rollInputTimer >= 0.15f && playerManager.animator.GetFloat("Vertical") >= 0.4f && !playerManager.playerLocomotion.isSprinting)
            {
                if (playerManager.animator.GetFloat("Vertical") <= 0.5f && !PlayerManager.Instance.playerLocomotion.startedSprinting)
                {
                    playerManager.animationManager.PlayTargetAnimation("Idle To Sprint", false);
                    PlayerManager.Instance.playerLocomotion.startedSprinting = true;
                    playerManager.animator.SetFloat("Vertical", 0f);
                    playerManager.playerLocomotion.currentSpeed = playerManager.playerLocomotion.GetSpeedFromAnimatorParameter();
                }
                else if (!PlayerManager.Instance.playerLocomotion.startedSprinting)
                {
                    playerManager.playerLocomotion.isSprinting = true;
                }
            }
            else if (playerManager.animator.GetFloat("Vertical") < 0.4f)
                playerManager.playerLocomotion.isSprinting = false;
        }
        else if (!sprintInput && !playerManager.isInteracting)
        {
            if (rollInputTimer > 0 && rollInputTimer < 0.15f)
            {
                playerManager.playerLocomotion.isSprinting = false;
                playerManager.playerLocomotion.isRolling = true;
            }

            rollInputTimer = 0f;
            playerManager.playerLocomotion.isSprinting = false;
        }
    }

    private void HandleWalkInput()
    {
        walkInput = playerControls.PlayerMovement.Walk.phase == UnityEngine.InputSystem.InputActionPhase.Started;
    }
}
