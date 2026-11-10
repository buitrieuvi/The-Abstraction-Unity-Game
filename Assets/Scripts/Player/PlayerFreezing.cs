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

        playerState.player.CameraManager.SetInputIsActiveCameras("cam3", false);
        playerState.player.CameraManager.SetInputIsActiveCameras("cam1", false);

        //playerState.player.InputManager.InputActions.Player.Disable();
        //playerState.player.InputManager.InputActions.UI.Enable();

        base.Enter();
    }

    public override void Exit()
    {
        playerState.player.InputManager.InputActions.Player.Move.Enable();

        playerState.player.CameraManager.SetInputIsActiveCameras("cam3", true);
        playerState.player.CameraManager.SetInputIsActiveCameras("cam1", true);

        //playerState.player.InputManager.InputActions.Player.Enable();
        //playerState.player.InputManager.InputActions.UI.Disable();

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
