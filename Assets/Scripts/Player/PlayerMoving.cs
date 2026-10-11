public class PlayerMoving : PlayerGrounded
{
    public PlayerMoving(PlayerState playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerState.player.IsMoving = true;
    }

    public override void Exit()
    {
        base.Exit();
        playerState.player.IsMoving = false;
    }

    public override void Update()
    {
        base.Update();
        OnMove();
    }
}
