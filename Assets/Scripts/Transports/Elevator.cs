using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Elevator : EventTrigger
{
    [SerializeField] private List<float> floors;

    public ElevatorView ElevatorView;

    public ElevatorController ElevatorController = new();

    public List<float> Floors;

    public void Awake()
    {
        //Floors.Add();

        ElevatorController.Elevator = this;

        OnTriggerEnterEvent += () =>
        {
            Debug.Log("Open");
            inputManager.InputActions.Player.Interact.started += HandleInteract;
        };

        OnTriggerExitEvent += () => 
        {
            Debug.Log("Close");
            inputManager.InputActions.Player.Interact.started -= HandleInteract;

            ElevatorView = null;
        };
    }

    public override void OnTriggerEnter(Collider other)
    {
        //base.OnTriggerEnter(other);
        //other.transform.SetParent(gameObject.transform);
    }

    public override void OnTriggerExit(Collider other)
    {
        //base.OnTriggerExit(other);
        //other.transform.SetParent(null);
    }

    public void HandleInteract(InputAction.CallbackContext _)
    {
        viewManager.LoadView<ElevatorView>(ElevatorController);
    }

    public void GoFloor(int f) 
    {
        transform.DOMoveY(16.65f * f, 5*f);
    }

}