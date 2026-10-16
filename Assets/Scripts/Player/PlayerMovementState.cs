using UnityEngine;

public class PlayerMovementState : IState
{
    protected readonly PlayerState playerState;
    protected float targetAngle;
    protected float angle;
    protected Vector3 moveDir;
    protected float turnSmoothTime = 0.1f;
    protected float turnSmoothVelocity;
    protected float speed;
    protected float modeAnimation;

    public PlayerMovementState(PlayerState playerState)
    {
        this.playerState = playerState;
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void HandleInput()
    {
        PlayerController player = playerState.player;
        player.Move = player.InputManager.InputActions.Player.Move.ReadValue<Vector2>();
        if (player.Move.sqrMagnitude > 0.0001f)
        {
            player.LastMove = player.Move;
        }
    }

    public virtual void PhysicsUpdate()
    {
    }

    public virtual void Update()
    {
        PlayerController player = playerState.player;
        player.IsGrounded = player.CharCtrl.isGrounded;
        player.DistanceToGround = CheckDistanceToGround();
        ApplyGravity();
        player.SetAnimationSpeed();
    }

    protected void OnMove()
    {
        //PlayerController player = playerState.player;
        //player.Speed = Mathf.MoveTowards(
        //    player.Speed,
        //    speed,
        //    player.CrouchTime * Time.deltaTime);

        //player.AnimationSpeed = Mathf.MoveTowards(
        //    player.AnimationSpeed,
        //    modeAnimation,
        //    player.CrouchTime * Time.deltaTime);

        //Camera mainCamera = Camera.main;
        //if (mainCamera == null)
        //{
        //    Debug.LogError("Player movement requires a camera tagged MainCamera.", player);
        //    return;
        //}

        //targetAngle = Mathf.Atan2(player.LastMove.x, player.LastMove.y) * Mathf.Rad2Deg
        //    + mainCamera.transform.eulerAngles.y;

        //angle = Mathf.SmoothDampAngle(
        //    player.CharCtrl.transform.eulerAngles.y,
        //    targetAngle,
        //    ref turnSmoothVelocity,
        //    turnSmoothTime);

        //if (player.Move.sqrMagnitude > 0.0001f)
        //{
        //    player.CharCtrl.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        //}

        //moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        //player.CharCtrl.Move(moveDir.normalized * player.Speed * Time.deltaTime);

        PlayerController player = playerState.player;

        player.Speed = Mathf.MoveTowards(
            player.Speed,
            speed,
            player.CrouchTime * Time.deltaTime
        );

        player.AnimationSpeed = Mathf.MoveTowards(
            player.AnimationSpeed,
            modeAnimation,
            player.CrouchTime * Time.deltaTime
        );

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Không tìm thấy MainCamera.", player);
            return;
        }

        // Tính hướng di chuyển theo camera
        targetAngle = Mathf.Atan2(
            player.LastMove.x,
            player.LastMove.y
        ) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;

        angle = Mathf.SmoothDampAngle(
            player.Rb.rotation.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            turnSmoothTime
        );

        // Xoay nhân vật
        if (player.Move.sqrMagnitude > 0.0001f)
        {
            player.Rb.MoveRotation(
                Quaternion.Euler(0f, angle, 0f)
            );
        }

        // Tính vector di chuyển
        moveDir = Quaternion.Euler(
            0f,
            targetAngle,
            0f
        ) * Vector3.forward;

        HandleStepOffset();

        // Di chuyển bằng Rigidbody
        Vector3 velocity = player.Rb.linearVelocity;

        velocity.x = moveDir.normalized.x * player.Speed;
        velocity.z = moveDir.normalized.z * player.Speed;

        player.Rb.linearVelocity = velocity;
    }
    protected void OnMoveCharCtrl()
    {
        PlayerController player = playerState.player;
        player.Speed = Mathf.MoveTowards(
            player.Speed,
            speed,
            player.CrouchTime * Time.deltaTime);

        player.AnimationSpeed = Mathf.MoveTowards(
            player.AnimationSpeed,
            modeAnimation,
            player.CrouchTime * Time.deltaTime);

        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Player movement requires a camera tagged MainCamera.", player);
            return;
        }

        targetAngle = Mathf.Atan2(player.LastMove.x, player.LastMove.y) * Mathf.Rad2Deg
            + mainCamera.transform.eulerAngles.y;

        angle = Mathf.SmoothDampAngle(
            player.CharCtrl.transform.eulerAngles.y,
            targetAngle,
            ref turnSmoothVelocity,
            turnSmoothTime);

        if (player.Move.sqrMagnitude > 0.0001f)
        {
            player.CharCtrl.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        player.CharCtrl.Move(moveDir.normalized * player.Speed * Time.deltaTime);
    }
    protected void EvaluateLocomotionState()
    {
        PlayerController player = playerState.player;
        bool hasInput = player.Move.sqrMagnitude > 0.0001f;

        if (!hasInput)
        {
            if (!player.IsIdling && !player.IsStoping)
            {
                playerState.ChangeState(playerState.stoping);
            }

            return;
        }

        if (player.IsCtrl)
        {
            if (!player.IsSlowing)
            {
                playerState.ChangeState(playerState.slowing);
            }

            return;
        }

        if (player.IsPressRunning)
        {
            if (!player.IsRunning)
            {
                playerState.ChangeState(playerState.running);
            }

            return;
        }

        if (!player.IsWalking)
        {
            playerState.ChangeState(playerState.walking);
        }
    }


    protected float CheckDistanceToGroundCharCtrl()
    {
        PlayerController player = playerState.player;
        Vector3 bottom = player.transform.position
            + Vector3.up * player.CharCtrl.center.y
            - Vector3.up * (player.CharCtrl.height / 2f - player.CharCtrl.skinWidth);

        float maxDistance = player.CharCtrl.skinWidth + 0.25f;
        if (Physics.Raycast(
                bottom,
                Vector3.down,
                out RaycastHit hit,
                maxDistance,
                player.GroundLayer,
                QueryTriggerInteraction.Ignore))
        {
            return hit.distance;
        }

        return -1f;
    }
    protected float CheckDistanceToGround()
    {
        PlayerController player = playerState.player;

        CapsuleCollider capsule =
            player.GetComponent<CapsuleCollider>();

        if (capsule == null)
            return -1f;

        Vector3 bottom = capsule.bounds.center
            - Vector3.up * (capsule.bounds.extents.y
            - capsule.radius);

        float maxDistance = 0.5f;

        if (Physics.Raycast(
            bottom,
            Vector3.down,
            out RaycastHit hit,
            maxDistance,
            player.GroundLayer,
            QueryTriggerInteraction.Ignore))
        {
            return hit.distance;
        }

        return -1f;
    }

    protected void ApplyGravity()
    {
        PlayerController player = playerState.player;

        player.IsGrounded = Physics.Raycast(
            player.Rb.position,
            Vector3.down,
            out _,
            1.1f,
            player.GroundLayer,
            QueryTriggerInteraction.Ignore
        );

        player.DistanceToGround = CheckDistanceToGround();
    }

    protected void ApplyGravityCharCtrl()
    {
        PlayerController player = playerState.player;
        if (player.CharCtrl.isGrounded && player.VerticalVelocity < 0f)
        {
            player.VerticalVelocity = -2f;
        }
        else
        {
            player.VerticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        player.CharCtrl.Move(Vector3.up * player.VerticalVelocity * Time.deltaTime);
    }

    public virtual void OntriggerEnter(Collider coll)
    {
    }

    public virtual void OntriggerExit(Collider coll)
    {
    }

    protected void HandleStepOffset()
    {
        PlayerController player = playerState.player;

        if (player.Rb == null || player.Move.sqrMagnitude < 0.0001f)
            return;

        if (!player.IsGrounded)
            return;

        CapsuleCollider capsule = player.GetComponent<CapsuleCollider>();
        if (capsule == null)
            return;

        const float maxStepHeight = 0.7f;
        const float checkDistance = 0.7f;
        const float skin = 0.003f;

        Vector3 direction = moveDir.normalized;
        if (direction.sqrMagnitude < 0.001f)
            return;

        Bounds bounds = capsule.bounds;

        // Vị trí chân nhân vật.
        float feetY = bounds.min.y;

        // Kiểm tra vật cản ở tầm thấp.
        Vector3 lowerOrigin = new Vector3(
            bounds.center.x,
            feetY + skin,
            bounds.center.z
        );

        if (!Physics.Raycast(
            lowerOrigin,
            direction,
            out RaycastHit lowerHit,
            checkDistance,
            player.GroundLayer,
            QueryTriggerInteraction.Ignore))
            return;

        // Không bước lên nếu vật cản cao hơn giới hạn.
        Vector3 upperOrigin = lowerOrigin
            + Vector3.up * maxStepHeight;

        if (Physics.Raycast(
            upperOrigin,
            direction,
            checkDistance,
            player.GroundLayer,
            QueryTriggerInteraction.Ignore))
            return;

        // Tìm mặt trên của bậc ở phía trước.
        Vector3 stepProbeOrigin = lowerHit.point
            + direction * 0.08f
            + Vector3.up * (maxStepHeight + 0.05f);

        if (!Physics.Raycast(
            stepProbeOrigin,
            Vector3.down,
            out RaycastHit stepHit,
            maxStepHeight + 0.1f,
            player.GroundLayer,
            QueryTriggerInteraction.Ignore))
            return;

        float stepHeight = stepHit.point.y - feetY;

        if (stepHeight <= skin || stepHeight > maxStepHeight)
            return;

        // Kiểm tra vị trí mới có đủ chỗ cho CapsuleCollider không.
        Vector3 lift = Vector3.up * (stepHeight + skin);

        Vector3 capsuleCenter = capsule.bounds.center + lift;
        float radius = Mathf.Max(0.01f, capsule.radius * Mathf.Max(
            capsule.transform.lossyScale.x,
            capsule.transform.lossyScale.z
        ));

        float halfHeight = Mathf.Max(
            radius,
            capsule.bounds.extents.y
        );

        Vector3 point1 = capsuleCenter + Vector3.up * (halfHeight - radius);
        Vector3 point2 = capsuleCenter - Vector3.up * (halfHeight - radius);

        bool blocked = Physics.CheckCapsule(
            point1,
            point2,
            radius * 0.95f,
            player.GroundLayer,
            QueryTriggerInteraction.Ignore
        );

        if (blocked)
            return;

        // Nâng Rigidbody trong bước vật lý.
        player.Rb.MovePosition(player.Rb.position + lift);
    }
}
