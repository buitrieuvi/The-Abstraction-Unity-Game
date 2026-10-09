public class PlayerFreezing : PlayerMovementState
{
    public PlayerFreezing(PlayerState playerState) : base(playerState)
    {

    }

    public override void Enter()
    {
        playerState.player.Input.InputActions.Player.Move.Disable();
        base.Enter();
    }

    public override void Exit()
    {
        playerState.player.Input.InputActions.Player.Move.Enable();
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if (playerState.player.Speed > 0.1f)
        {
            OnMove();
        }
    }
}
