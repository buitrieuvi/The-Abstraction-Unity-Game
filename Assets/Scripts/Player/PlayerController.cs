using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public sealed class PlayerController : MonoBehaviour
{
    [Inject] public InputManager Input { get; private set; }
    [Inject] public LayerController Layer { get; private set; }
    //[Inject] public GameDataManager GameData { get; private set; }

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
    }

    private void Start()
    {
        Input.InputActions.Player.Interact.started += HandleInteract;
        Input.InputActions.Player.Alt.started += HandleAlt;
        Input.InputActions.Player.Ctrl.performed += HandleControl;
        Input.InputActions.Player.Ctrl.canceled += HandleControl;
        Input.InputActions.Player.Sprint.performed += HandleSprint;
        Input.InputActions.Player.Sprint.canceled += HandleSprint;
    }

    private void OnDestroy()
    {
        if (Input?.InputActions == null)
        {
            return;
        }

        Input.InputActions.Player.Interact.started -= HandleInteract;
        Input.InputActions.Player.Alt.started -= HandleAlt;
        Input.InputActions.Player.Ctrl.performed -= HandleControl;
        Input.InputActions.Player.Ctrl.canceled -= HandleControl;
        Input.InputActions.Player.Sprint.performed -= HandleSprint;
        Input.InputActions.Player.Sprint.canceled -= HandleSprint;
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
