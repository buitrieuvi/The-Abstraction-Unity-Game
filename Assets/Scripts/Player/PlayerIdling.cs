using UnityEngine;

public class PlayerIdling : PlayerGrounded
{
    public PlayerIdling(PlayerState playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    
    }

    public override void Enter()
    {
        base.Enter();
        playerState.player.IsIdling = true;
        playerState.player.Speed = 0f;

        playerState.player.AnimationSpeed = 0f;
        modeAnimation = 0f;
    }

    public override void Exit()
    {
        base.Exit();
        playerState.player.IsIdling = false;
    }

    public override void Update()
    {
        base.Update();

        EvaluateLocomotionState();
    }
}
