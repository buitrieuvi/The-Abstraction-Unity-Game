using UnityEngine;

public class PlayerStoping : PlayerMoving
{
    public PlayerStoping(PlayerState playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerState.player.IsStoping = true;
        speed = 0;
    }

    public override void Exit()
    {
        base.Exit();
        playerState.player.IsStoping = false;
    }

    public override void Update()
    {
        base.Update();

        if (playerState.player.Move.sqrMagnitude > 0.0001f)
        {
            EvaluateLocomotionState();
            return;
        }

        if (playerState.player.Speed < 0.001f)
        {
            playerState.ChangeState(playerState.idling);
        }
    }
}
