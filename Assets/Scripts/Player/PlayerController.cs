using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
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

    private Transform heldCubeTransform = null; // Lưu giữ đối tượng Cube đang cầm
    private Rigidbody heldCubeRb = null;

    [Header("Cài đặt xoay khi cầm")]
    public float rotationSpeed = 50f; // Tốc độ xoay của Cube (độ/giây)

    private void Awake()
    {
        CharCtrl = GetComponent<CharacterController>();

        Movement = new PlayerState(this);
        Movement.ChangeState(Movement.idling);
        Animator = GetComponentInChildren<Animator>();
        Rb = GetComponent<Rigidbody>();

        InputManager.SetCursorVisible(false);

        InputManager.InputActions.Player.Attack.performed += (e) =>
        {
            // THƯỜNG TRƯỜNG HỢP 1: ĐANG CẦM CUBE -> THÌ NÉM ĐI
            if (heldCubeTransform != null)
            {
                ThrowCube();
                return;
            }

            // TRƯỜNG HỢP 2: CHƯA CẦM GÌ -> THÌ BẮN RAYCAST ĐỂ NHẶT
            Vector3 screenCenterPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;
                Cube cb = objectHit.GetComponent<Cube>();

                if (cb != null)
                {
                    PickupCube(cb);
                }
            }
        };


        InputManager.InputActions.Player.Inventory.performed += OnInventoryPerformed;
        InputManager.InputActions.Player.Esc.performed += OnMenuPerformed;

        destroyCancellationToken.Register(() =>
        {

            if (InputManager.InputActions != null) 
            {
                InputManager.InputActions.Player.Inventory.performed -= OnInventoryPerformed;
            }
        });

        destroyCancellationToken.Register(() =>
        {

            if (InputManager.InputActions != null)
            {
                InputManager.InputActions.Player.Esc.performed -= OnMenuPerformed;
            }
        });
    }

    

    private async void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        var v = await ViewManager.ShowView<InventoryView>(DataManager.PlayerInventory);
        if (v != null)
        {
            Movement.ChangeState(Movement.freezing);
        }
        else 
        {
            Movement.ChangeState(Movement.idling);
        }
    }

    private async void OnMenuPerformed(InputAction.CallbackContext context)
    {
        var v = await ViewManager.ShowView<MenuView>(DataManager.PlayerInventory);
        if (v != null)
        {
            Movement.ChangeState(Movement.freezing);
        }
        else
        {
            Movement.ChangeState(Movement.idling);
        }
    }

    private void Start()
    {
        InputManager.InputActions.Player.Interact.started += HandleInteract;
        InputManager.InputActions.Player.Alt.started += HandleAlt;
        InputManager.InputActions.Player.Ctrl.performed += HandleControl;
        InputManager.InputActions.Player.Ctrl.canceled += HandleControl;
        InputManager.InputActions.Player.Sprint.performed += HandleSprint;
        InputManager.InputActions.Player.Sprint.canceled += HandleSprint;

        InputManager.SetCursorVisible(false);
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


        // Nếu đang cầm Cube, liên tục xoay nó quanh trục Y (hoặc trục tùy ý) theo thời gian thực
        if (heldCubeTransform != null)
        {
            // Xoay theo không gian cục bộ (Local) của Cube hoặc Camera
            heldCubeTransform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            heldCubeTransform.Rotate(Vector3.left * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void FixedUpdate()
    {
        Movement.PhysicsUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

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

    void PickupCube(Cube cb)
    {

        heldCubeTransform = cb.transform;
        heldCubeRb = cb.Rb;

        // 1. Tắt Rigidbody để vật thể không bị rơi
        if (heldCubeRb != null)
        {
            heldCubeRb.isKinematic = true;
            heldCubeRb.useGravity = false;
        }

        // 2. Gắn làm con của Camera để nó xoay theo hướng nhìn của người chơi
        heldCubeTransform.SetParent(Camera.main.transform);

        // 3. Đặt vị trí ở trước mặt
        heldCubeTransform.localPosition = new Vector3(0f, 0f, 2f);
        heldCubeTransform.localRotation = Quaternion.identity;

        heldCubeRb.transform.DOScale(0.1f, 0);
    }

    void ThrowCube()
    {
        heldCubeRb.transform.DOScale(0.5f, 0);

        // 1. Tách khỏi Camera (hết làm con)
        heldCubeTransform.SetParent(null);

        // 2. Bật lại Rigidbody và trọng lực
        if (heldCubeRb != null)
        {
            heldCubeRb.isKinematic = false;
            heldCubeRb.useGravity = true;

            // 3. Dùng lực đẩy (AddForce) hoặc vận tốc (velocity) để ném cube bay về phía camera đang nhìn
            float throwForce = 15f; // Bạn có thể chỉnh độ mạnh của lực ném tại đây
            heldCubeRb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
        }

        // Xóa trạng thái đang cầm
        heldCubeTransform = null;
        heldCubeRb = null;
    }
}
