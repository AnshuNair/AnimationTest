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

    public void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        playerManager.animationManager.UpdateAnimatorValues(0, moveAmount, playerManager.playerLocomotion.isSprinting);
    }
}
