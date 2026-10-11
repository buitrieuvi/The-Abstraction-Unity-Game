using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>Exploration camera inspired by ZZZ. Attach to the scene's Main Camera.</summary>
[DefaultExecutionOrder(50)]
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public sealed class ThirdPersonCameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private PlayerController player;
    [SerializeField] private float targetHeight = 1.25f;

    [Header("Framing")]
    [SerializeField] private Vector3 shoulderOffset = new Vector3(0.35f, 0f, 0f);
    [SerializeField, Min(0.1f)] private float distance = 4.5f;
    [SerializeField] private Vector2 zoomLimits = new Vector2(2.5f, 6.5f);
    [SerializeField, Min(0f)] private float zoomStep = 0.5f;
    [SerializeField, Range(20f, 100f)] private float fieldOfView = 48f;
    [SerializeField, Range(0f, 15f)] private float runningFovBoost = 5f;
    [SerializeField] private Vector3 followDamping = new Vector3(0.12f, 0.25f, 0.15f);

    [Header("Orbit")]
    [SerializeField] private float startingPitch = 12f;
    [SerializeField] private Vector2 pitchLimits = new Vector2(-25f, 65f);
    [SerializeField, Min(0f)] private float mouseSensitivity = 0.12f;
    [SerializeField, Min(0f)] private float gamepadSensitivity = 150f;
    [SerializeField, Min(0.001f)] private float rotationSmoothTime = 0.045f;
    [SerializeField] private bool invertY;

    [Header("Obstacles")]
    [Tooltip("Exclude the Player layer. Environment colliders prevent camera clipping.")]
    [SerializeField] private LayerMask obstacleLayers = Physics.DefaultRaycastLayers & ~(1 << 3);
    [SerializeField, Range(0.05f, 0.5f)] private float collisionRadius = 0.2f;

    private GameObject rig;
    private Transform orbitTarget;
    private CinemachineCamera virtualCamera;
    private CinemachineThirdPersonFollow follow;
    private InputAction zoomInput;
    private InputAction recenterInput;
    private InputAction releaseInput;
    private InputAction captureInput;
    private float yaw;
    private float pitch;
    private float smoothYaw;
    private float smoothPitch;
    private float yawVelocity;
    private float pitchVelocity;
    private float desiredDistance;
    private bool cursorCaptured;
    private bool previousCursorVisible;
    private CursorLockMode previousCursorLock;

    private void Awake()
    {
        zoomInput = new InputAction("Camera Zoom", InputActionType.Value, "<Mouse>/scroll/y");
        recenterInput = new InputAction("Recenter Camera", InputActionType.Button, "<Mouse>/middleButton");
        recenterInput.AddBinding("<Gamepad>/rightStickPress");
        releaseInput = new InputAction("Release Camera Cursor", InputActionType.Button, "<Keyboard>/escape");
        captureInput = new InputAction("Capture Camera Cursor", InputActionType.Button, "<Mouse>/rightButton");
    }

    private void OnEnable()
    {
        zoomInput.Enable();
        recenterInput.Enable();
        releaseInput.Enable();
        captureInput.Enable();
        if (rig != null)
            rig.SetActive(true);
    }

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Assign a PlayerController to the third-person camera.", this);
            enabled = false;
            return;
        }

        CinemachineBrain brain = GetComponent<CinemachineBrain>();
        if (brain == null)
            brain = gameObject.AddComponent<CinemachineBrain>();
        brain.UpdateMethod = CinemachineBrain.UpdateMethods.LateUpdate;
        brain.BlendUpdateMethod = CinemachineBrain.BrainUpdateMethods.LateUpdate;

        // A separate root keeps orbit rotation independent of both player and output camera.
        rig = new GameObject("Third Person Camera Rig");
        SceneManager.MoveGameObjectToScene(rig, gameObject.scene);
        orbitTarget = new GameObject("Camera Orbit Target").transform;
        orbitTarget.SetParent(rig.transform, false);
        yaw = smoothYaw = player.transform.eulerAngles.y;
        pitch = smoothPitch = Mathf.Clamp(startingPitch, pitchLimits.x, pitchLimits.y);
        desiredDistance = Mathf.Clamp(distance, zoomLimits.x, zoomLimits.y);
        UpdateOrbitTarget();

        GameObject cameraObject = new GameObject("Exploration Cinemachine Camera");
        cameraObject.SetActive(false);
        cameraObject.transform.SetParent(rig.transform, false);
        virtualCamera = cameraObject.AddComponent<CinemachineCamera>();
        virtualCamera.Follow = orbitTarget;
        virtualCamera.Lens.FieldOfView = fieldOfView;
        virtualCamera.Lens.NearClipPlane = 0.1f;
        virtualCamera.Lens.FarClipPlane = GetComponent<Camera>().farClipPlane;
        follow = cameraObject.AddComponent<CinemachineThirdPersonFollow>();
        follow.CameraDistance = desiredDistance;
        ApplyRigSettings();
        cameraObject.SetActive(true);
        CaptureCursor();
    }

    private void LateUpdate()
    {
        if (player == null || virtualCamera == null)
            return;

        if (releaseInput.WasPressedThisFrame())
            ReleaseCursor();
        else if (captureInput.WasPressedThisFrame() && Application.isFocused)
            CaptureCursor();

        float deltaTime = Time.deltaTime;
        if (deltaTime <= 0f)
            return;

        if (cursorCaptured && Cursor.lockState == CursorLockMode.Locked && Application.isFocused)
        {
            InputAction lookAction = player.InputManager?.InputActions?.Player.Look;
            if (lookAction != null && lookAction.enabled)
            {
                Vector2 look = lookAction.ReadValue<Vector2>();
                // Mouse is displacement per frame; a stick is angular velocity.
                float sensitivity = lookAction.activeControl?.device is Gamepad
                    ? gamepadSensitivity * deltaTime : mouseSensitivity;
                yaw += look.x * sensitivity;
                pitch += look.y * sensitivity * (invertY ? 1f : -1f);
            }

            if (recenterInput.WasPressedThisFrame())
            {
                yaw = player.transform.eulerAngles.y;
                pitch = startingPitch;
            }

            float scroll = zoomInput.ReadValue<float>();
            if (Mathf.Abs(scroll) > 0.01f)
                desiredDistance -= Mathf.Sign(scroll) * zoomStep;
        }

        yaw = Mathf.Repeat(yaw, 360f);
        pitch = Mathf.Clamp(pitch, pitchLimits.x, pitchLimits.y);
        desiredDistance = Mathf.Clamp(desiredDistance, zoomLimits.x, zoomLimits.y);
        smoothYaw = Mathf.SmoothDampAngle(smoothYaw, yaw, ref yawVelocity, rotationSmoothTime);
        smoothPitch = Mathf.SmoothDampAngle(smoothPitch, pitch, ref pitchVelocity, rotationSmoothTime);
        UpdateOrbitTarget();
        ApplyRigSettings();

        float blend = 1f - Mathf.Exp(-8f * deltaTime);
        follow.CameraDistance = Mathf.Lerp(follow.CameraDistance, desiredDistance, blend);
        float runningBlend = player.IsRunning
            ? Mathf.Clamp01(player.Speed / Mathf.Max(0.01f, player.SpeedRunning)) : 0f;
        virtualCamera.Lens.FieldOfView = Mathf.Lerp(
            virtualCamera.Lens.FieldOfView, fieldOfView + runningFovBoost * runningBlend, blend);
    }

    private void UpdateOrbitTarget()
    {
        orbitTarget.SetPositionAndRotation(
            player.transform.position + Vector3.up * targetHeight,
            Quaternion.Euler(smoothPitch, smoothYaw, 0f));
    }

    private void ApplyRigSettings()
    {
        follow.ShoulderOffset = shoulderOffset;
        follow.VerticalArmLength = 0.15f;
        follow.CameraSide = 1f;
        follow.Damping = followDamping;
        follow.AvoidObstacles = new CinemachineThirdPersonFollow.ObstacleSettings
        {
            Enabled = true,
            CollisionFilter = obstacleLayers,
            IgnoreTag = "Player",
            CameraRadius = collisionRadius,
            DampingIntoCollision = 0f,
            DampingFromCollision = 0.35f
        };
    }

    private void CaptureCursor()
    {
        if (player != null && player.InputManager != null && player.InputManager.IsInventoryOpen)
        {
            return;
        }
        if (!cursorCaptured)
        {
            previousCursorVisible = Cursor.visible;
            previousCursorLock = Cursor.lockState;
        }
        cursorCaptured = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void ReleaseCursor()
    {
        if (!cursorCaptured)
            return;
        cursorCaptured = false;
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;
    }

    private void OnApplicationFocus(bool focused)
    {
        if (!focused)
            ReleaseCursor();
    }

    private void OnDisable()
    {
        zoomInput?.Disable();
        recenterInput?.Disable();
        releaseInput?.Disable();
        captureInput?.Disable();
        ReleaseCursor();
        if (rig != null)
            rig.SetActive(false);
    }

    private void OnDestroy()
    {
        zoomInput?.Dispose();
        recenterInput?.Dispose();
        releaseInput?.Dispose();
        captureInput?.Dispose();
        if (rig != null)
            Destroy(rig);
    }

    private void OnValidate()
    {
        zoomLimits.x = Mathf.Max(0.1f, zoomLimits.x);
        zoomLimits.y = Mathf.Max(zoomLimits.x, zoomLimits.y);
        pitchLimits.x = Mathf.Clamp(pitchLimits.x, -80f, 80f);
        pitchLimits.y = Mathf.Clamp(pitchLimits.y, pitchLimits.x, 80f);
        followDamping = Vector3.Max(Vector3.zero, followDamping);
    }
}
