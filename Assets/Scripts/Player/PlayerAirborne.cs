public class PlayerAirborne : PlayerMovementState
{
    public PlayerAirborne(PlayerState playerState) : base(playerState)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerState.player.IsAirbone = true;
    }

    public override void Exit()
    {
        base.Exit();
        playerState.player.IsAirbone = false;
    }

}
