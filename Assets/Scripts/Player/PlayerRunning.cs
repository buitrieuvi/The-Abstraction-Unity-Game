using UnityEngine;

public class PlayerRunning : PlayerMoving
{
    public PlayerRunning(PlayerState playerMovementStateMachine) : base(playerMovementStateMachine)
    {

    }

    public override void Enter()
    {
        base.Enter();
        playerState.player.IsRunning = true;
        speed = playerState.player.SpeedRunning;

        modeAnimation = 2f;
    }

    public override void Exit()
    {
        base.Exit();
        playerState.player.IsRunning = false;
    }

    public override void Update()
    {

        base.Update();

        EvaluateLocomotionState();
    }
}
