public class PlayerFreezing : PlayerMovementState
{
    public PlayerFreezing(PlayerState playerState) : base(playerState)
    {

    }

    public override void Enter()
    {
        playerState.player.InputManager.InputActions.Player.Move.Disable();
        playerState.player.InputManager.SetCursorVisible(false);
        base.Enter();
    }

    public override void Exit()
    {
        playerState.player.InputManager.InputActions.Player.Move.Enable();
        playerState.player.InputManager.SetCursorVisible(true);
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
