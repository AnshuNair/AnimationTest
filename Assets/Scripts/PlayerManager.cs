using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public GameObject playerModel;
    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public PlayerLocomotion playerLocomotion;
    [HideInInspector] public CameraManager cameraManager;
    [HideInInspector] public AnimationManager animationManager;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody playerRigidbody;

    [Header("Player Interaction Flags")]
    public bool isInteracting;
    public bool isUsingRootMotion;

    private float characterIdleTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
        playerLocomotion = PlayerLocomotion.Instance;
        cameraManager = CameraManager.Instance;
        animator = playerModel.GetComponent<Animator>();
        playerRigidbody = playerModel.GetComponent<Rigidbody>();
        animationManager = playerModel.GetComponent<AnimationManager>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        isInteracting = animator.GetBool("isInteracting");
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);
        CountIdleTime();
        playerLocomotion.isWalking = inputManager.walkInput && inputManager.moveAmount > 0f && !playerLocomotion.startedSprinting;
    }

    private void CountIdleTime()
    {
        if (!isInteracting && Mathf.RoundToInt(animator.GetFloat("Horizontal")) == 0 && Mathf.RoundToInt(animator.GetFloat("Vertical")) == 0)
        {
            characterIdleTime += Time.deltaTime;
        }
        else
        {
            characterIdleTime = 0f;
        }
        if (characterIdleTime >= 10f)
        {
            int fillerAnim = Random.Range(0, 3);
            if (fillerAnim == 0)
                animationManager.PlayTargetAnimation("Idle_filler", false);
            else if (fillerAnim == 1)
                animationManager.PlayTargetAnimation("Idle_filler_2", false);
            else
                animationManager.PlayTargetAnimation("Idle_filler_3", false);
            characterIdleTime = 0f;
        }
    }
}
