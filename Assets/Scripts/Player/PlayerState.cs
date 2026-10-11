public class PlayerState : StateMachine
{
    public PlayerController player { get; }

    public PlayerFreezing freezing { get; }
    public PlayerStoping stoping { get; }
    public PlayerIdling idling { get; }
    public PlayerSlowing slowing { get; }
    public PlayerWalking walking { get; }
    public PlayerRunning running { get; }
    public PlayerFall falling { get; }

    public PlayerState(PlayerController player)
    {
        this.player = player;

        freezing = new PlayerFreezing(this);
        idling = new PlayerIdling(this);
        slowing = new PlayerSlowing(this);
        walking = new PlayerWalking(this);
        running = new PlayerRunning(this);
        stoping = new PlayerStoping(this);
        falling = new PlayerFall(this);

        //player.Input.SetCursorVisible(false);
    }

}
