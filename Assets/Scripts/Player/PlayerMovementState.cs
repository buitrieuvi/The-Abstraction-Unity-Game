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

    protected float CheckDistanceToGround()
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

    protected void ApplyGravity()
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
}
