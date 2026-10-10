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

    public CharacterController CharCtrl { get; private set; }
    public PlayerState Movement { get; private set; }
    public EventTrigger CurrentTrigger { get; private set; }

    private readonly List<EventTrigger> _activeTriggers = new();

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
        UnityEngine.Animator.StringToHash("speed");

    private void Awake()
    {
        CharCtrl = GetComponent<CharacterController>();
        Movement = new PlayerState(this);
        Movement.ChangeState(Movement.idling);
        Animator = GetComponentInChildren<Animator>();

        InputManager.SetCursorVisible(false);


        InputManager.InputActions.Player.Inventory.performed += OnInventoryPerformed;
        destroyCancellationToken.Register(() =>
        {

            if (InputManager.InputActions != null) 
            {
                InputManager.InputActions.Player.Inventory.performed -= OnInventoryPerformed;
            }

            InputManager.IsInventoryOpen = false;
        });
    }


    private void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        if (!InputManager.IsInventoryOpen)
        {
            Movement.ChangeState(Movement.freezing);
        }
        else
        {
            Movement.ChangeState(Movement.idling);
        }

        ViewManager.LoadView<InventoryView>(DataManager.PlayerInventory);
        InputManager.IsInventoryOpen = true;
    }

    private void Start()
    {
        InputManager.InputActions.Player.Interact.started += HandleInteract;
        InputManager.InputActions.Player.Alt.started += HandleAlt;
        InputManager.InputActions.Player.Ctrl.performed += HandleControl;
        InputManager.InputActions.Player.Ctrl.canceled += HandleControl;
        InputManager.InputActions.Player.Sprint.performed += HandleSprint;
        InputManager.InputActions.Player.Sprint.canceled += HandleSprint;
    }

    private void OnDestroy()
    {
        if (InputManager?.InputActions == null)
        {
            return;
        }

        InputManager.InputActions.Player.Interact.started -= HandleInteract;
        InputManager.InputActions.Player.Alt.started -= HandleAlt;
        InputManager.InputActions.Player.Ctrl.performed -= HandleControl;
        InputManager.InputActions.Player.Ctrl.canceled -= HandleControl;
        InputManager.InputActions.Player.Sprint.performed -= HandleSprint;
        InputManager.InputActions.Player.Sprint.canceled -= HandleSprint;
    }

    private void Update()
    {
        Movement.HandleInput();
        Movement.Update();
    }

    private void FixedUpdate()
    {
        Movement.PhysicsUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        Movement.OntriggerEnter(other);

        EventTrigger trigger = other.GetComponentInParent<EventTrigger>();
        if (trigger == null || _activeTriggers.Contains(trigger))
        {
            return;
        }

        _activeTriggers.Add(trigger);
        CurrentTrigger = trigger;
        trigger.TriggerEnter();
    }

    private void OnTriggerExit(Collider other)
    {
        Movement.OntriggerExit(other);

        EventTrigger trigger = other.GetComponentInParent<EventTrigger>();
        if (trigger == null || !_activeTriggers.Remove(trigger))
        {
            return;
        }

        trigger.TriggerExit();
        CurrentTrigger = _activeTriggers.Count > 0
            ? _activeTriggers[_activeTriggers.Count - 1]
            : null;
    }

    public void SetAnimationSpeed()
    {
        if (Animator != null)
        {
            Animator.SetFloat(AnimationSpeedParameter, AnimationSpeed);
        }
    }

    private void HandleInteract(InputAction.CallbackContext _)
    {
        CurrentTrigger?.OnInteract();
    }

    private void HandleAlt(InputAction.CallbackContext _)
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
