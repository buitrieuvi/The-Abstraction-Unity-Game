using UnityEngine;

public class PlayerWalking : PlayerMoving
{
    public PlayerWalking(PlayerState playerMovementStateMachine) : base(playerMovementStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        playerState.player.IsWalking = true;

        speed = playerState.player.SpeedWalking;
        modeAnimation = 1f;
    }

    public override void Exit()
    {
        base.Exit();
        playerState.player.IsWalking = false;
    }

    public override void Update()
    {
        base.Update();

        EvaluateLocomotionState();
    }
}
