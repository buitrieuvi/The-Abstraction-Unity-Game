using DG.Tweening;
using NUnit.Framework;
using R3.Triggers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using R3;


public class Elevator : EventTrigger
{
    public ElevatorController ElevatorController = new();
    private bool isIn;

    public void Awake()
    {
        OnTriggerEnterEvent += () =>
        {
            inputManager.InputActions.Player.Interact.started += HandleInteract;
        };

        OnTriggerExitEvent += () => 
        {
            inputManager.InputActions.Player.Interact.started -= HandleInteract;
        };
    }

    public void Start()
    {

    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        ElevatorController.Elevator = this;

        other.transform.SetParent(transform);
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        ElevatorController.Elevator = null;

        other.transform.SetParent(null);

    }

    public async void HandleInteract(InputAction.CallbackContext _)
    {
        ElevatorController.ElevatorView = (ElevatorView)await viewManager.ShowView<ElevatorView>(ElevatorController);
    }

}