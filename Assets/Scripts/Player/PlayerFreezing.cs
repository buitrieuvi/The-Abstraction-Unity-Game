using Unity.Cinemachine;
using UnityEngine;

public class PlayerFreezing : PlayerMovementState
{
    public PlayerFreezing(PlayerState playerState) : base(playerState)
    {

    }

    public override void Enter()
    {
        playerState.player.InputManager.InputActions.Player.Move.Disable();
        playerState.player.InputManager.SetCursorVisible(true);
        playerState.player.CameraManager.SetInputIsActiveCameras("cam3", false);
        base.Enter();
    }

    public override void Exit()
    {
        playerState.player.InputManager.InputActions.Player.Move.Enable();
        playerState.player.InputManager.SetCursorVisible(false);
        playerState.player.CameraManager.SetInputIsActiveCameras("cam3", true);
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
