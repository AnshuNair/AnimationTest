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
    [HideInInspector] public float sprintInputTimer;
    [HideInInspector] public float spaceLetGoTimer;
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
        HandleSprintInput();
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

    private void HandleSprintInput()
    {
        sprintInput = playerControls.PlayerMovement.Sprint.phase == UnityEngine.InputSystem.InputActionPhase.Started;

        if (sprintInput && !playerManager.isInteracting)
        {
            rollInputTimer += Time.deltaTime;
            if (rollInputTimer >= 0.15f && playerManager.animator.GetFloat("Vertical") >= 0.4f)
            {
                if (!playerManager.playerLocomotion.isSprinting)
                {
                    if (!PlayerManager.Instance.playerLocomotion.startedSprinting)
                    {
                        playerManager.animationManager.PlayTargetAnimation("Idle To Sprint", false);
                        PlayerManager.Instance.playerLocomotion.startedSprinting = true;
                        playerManager.animator.SetFloat("Vertical", 0f);
                        playerManager.playerLocomotion.currentSpeed = playerManager.playerLocomotion.GetSpeedFromAnimatorParameter();
                    }
                }
                else
                {
                    sprintInputTimer += Time.deltaTime;
                    if (playerControls.PlayerMovement.Sprint.triggered)
                    {
                        playerManager.animationManager.PlayTargetAnimation("Running Jump", true);
                        playerManager.playerLocomotion.isSprinting = false;
                        sprintInputTimer = 0f;
                        rollInputTimer = 0f;
                        spaceLetGoTimer = 0f;
                    }
                }
            }
            else if (playerManager.animator.GetFloat("Vertical") < 0.4f)
                playerManager.playerLocomotion.isSprinting = false;
        }
        else if (!sprintInput && !playerManager.isInteracting)
        {
            spaceLetGoTimer += Time.deltaTime;
            if (spaceLetGoTimer >= 0.25f)
            {
                if (!playerManager.playerLocomotion.isSprinting && rollInputTimer > 0 && rollInputTimer < 0.15f)
                {
                    playerManager.playerLocomotion.isSprinting = false;
                    playerManager.playerLocomotion.isRolling = true;
                }

                if (sprintInputTimer >= 1f)
                {
                    playerManager.animationManager.PlayTargetAnimation("Run To Stop", true, true);
                }
                playerManager.playerLocomotion.isSprinting = false;
                sprintInputTimer = 0f;
                rollInputTimer = 0f;
                spaceLetGoTimer = 0f;
            }
        }
    }

    private void HandleWalkInput()
    {
        walkInput = playerControls.PlayerMovement.Walk.phase == UnityEngine.InputSystem.InputActionPhase.Started;
    }
}
