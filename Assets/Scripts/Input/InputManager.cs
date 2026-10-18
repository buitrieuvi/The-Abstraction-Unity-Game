using System;
using UnityEngine;
using Zenject;

public sealed class InputManager : IInitializable, IDisposable
{
    public bool IsInventoryOpen { get; set; }
    public InputSystem_Actions InputActions { get; private set; } = new InputSystem_Actions();

    public void Initialize() => InputActions.Enable();

    public void Dispose()
    {
        InputActions?.Disable();
        InputActions?.Dispose();
        InputActions = null;
    }

    public void SetCursorVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
        Cursor.lockState = isVisible
            ? CursorLockMode.None
            : CursorLockMode.Locked;
    }
}
