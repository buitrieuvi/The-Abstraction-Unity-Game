
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerController : MonoBehaviour
{
    [Inject] public InputManager InputManager { get; private set; }
    [Inject] public LayerController Layer { get; private set; }
    [Inject] public ViewManager ViewManager { get; set; }
    [Inject] public DataManager DataManager { get; set; }
    [Inject] public CameraManager CameraManager { get; set; }

    public CharacterController CharCtrl { get; private set; }
    public Rigidbody Rb { get; private set; }
    public PlayerState Movement { get; private set; }

    public EventTrigger CurrentTrigger { get; private set; }

    [Header("Interaction")]
    public EventTrigger Handle;
    public EventTrigger HandleHover;
    public float InteractionDistance = 5f;

    [Header("Movement")]
    public Vector2 Move;
    public Vector2 LastMove;
    public bool IsRunningMode;
    public bool IsPressRunning;
    public bool IsCtrl;
    public bool IsAlt;
    public float CrouchTime = 10f;
    public float SpeedSlowing = 2f;
    public float SpeedWalking = 5f;
    public float SpeedRunning = 8f;
    public float Speed;
    public bool IsGrounded;
    public bool IsIdling;
    public bool IsWalking;
    public bool IsSlowing;
    public bool IsRunning;
    public bool IsMoving;
    public bool IsStoping;
    public float DistanceToGround;
    public LayerMask GroundLayer = Physics.DefaultRaycastLayers;

    public float VerticalVelocity;
    public bool IsAirbone;
    public bool IsFalling;

    [Header("Animation")]
    public float AnimationSpeed;
    public Animator Animator;

    private static readonly int AnimationSpeedParameter =
        Animator.StringToHash("speed");

    private void Awake()
    {
        CharCtrl = GetComponent<CharacterController>();
        Rb = GetComponent<Rigidbody>();
        Animator = GetComponentInChildren<Animator>();

        Movement = new PlayerState(this);
        Movement.ChangeState(Movement.idling);
    }

    private void OnEnable()
    {
        var player = InputManager.InputActions.Player;

        player.Inventory.performed += OnInventoryPerformed;
        player.Esc.performed += OnMenuPerformed;

        player.Interact.started += HandleInteract;
        player.Alt.started += HandleAlt;
        player.Ctrl.performed += HandleControl;
        player.Ctrl.canceled += HandleControl;
        player.Sprint.performed += HandleSprint;
        player.Sprint.canceled += HandleSprint;

        player.Attack.performed += HandleAttack;
    }

    private void OnDisable()
    {
        if (InputManager?.InputActions == null)
            return;

        var player = InputManager.InputActions.Player;

        player.Inventory.performed -= OnInventoryPerformed;
        player.Esc.performed -= OnMenuPerformed;

        player.Interact.started -= HandleInteract;
        player.Alt.started -= HandleAlt;
        player.Ctrl.performed -= HandleControl;
        player.Ctrl.canceled -= HandleControl;
        player.Sprint.performed -= HandleSprint;
        player.Sprint.canceled -= HandleSprint;

        player.Attack.performed -= HandleAttack;

        HandleHover?.OnTriggerHoverExit();
        HandleHover = null;
    }

    private void Update()
    {
        Movement.HandleInput();
        Movement.Update();
    }

    private void FixedUpdate()
    {
        Movement.PhysicsUpdate();
        UpdateInteractionRaycast();
    }

    // ==================== INTERACTION ====================

    private void UpdateInteractionRaycast()
    {
        if (Camera.main == null)
            return;

        Vector3 screenCenterPoint = new Vector3(
            Screen.width / 2f,
            Screen.height / 2f,
            0f
        );

        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        EventTrigger target = null;

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            InteractionDistance))
        {
            target = hit.transform.GetComponentInParent<EventTrigger>();
        }

        if (HandleHover == target)
            return;

        HandleHover?.OnTriggerHoverExit();

        HandleHover = target;

        HandleHover?.OnTriggerHoverEnter();
    }

    private void HandleInteract(InputAction.CallbackContext context)
    {
        HandleHover?.OnInteracted();
    }

    private void HandleAttack(InputAction.CallbackContext context)
    {
        // Throw current object
        if (Handle != null)
        {
            Handle.OnThrowed();
            Handle = null;
            return;
        }

        if (HandleHover == null)
            return;

        // Pick object
        if (HandleHover.CompareTag("cube"))
        {
            Handle = HandleHover;
        }

        HandleHover.OnPicked();
    }

    // ==================== VIEW ====================

    private async void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        var v = await ViewManager.ShowView<InventoryView>(
            DataManager.PlayerInventory
        );

        IsView(v);
    }

    private async void OnMenuPerformed(InputAction.CallbackContext context)
    {
        var v = await ViewManager.ShowView<MenuView>(
            DataManager.PlayerInventory
        );

        IsView(v);
    }

    private void IsView(ViewBase v)
    {
        if (v != null)
            Movement.ChangeState(Movement.freezing);
        else
            Movement.ChangeState(Movement.idling);
    }

    // ==================== MOVEMENT ====================

    public void SetAnimationSpeed()
    {
        if (Animator != null)
        {
            Animator.SetFloat(
                AnimationSpeedParameter,
                AnimationSpeed
            );
        }
    }

    private void HandleAlt(InputAction.CallbackContext context)
    {
        IsAlt = !IsAlt;
    }

    private void HandleControl(InputAction.CallbackContext context)
    {
        IsCtrl = context.ReadValueAsButton();
    }

    private void HandleSprint(InputAction.CallbackContext context)
    {
        IsPressRunning = IsRunningMode || context.ReadValueAsButton();
    }
}