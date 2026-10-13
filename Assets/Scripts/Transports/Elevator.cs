using DG.Tweening;
using NUnit.Framework;
using R3.Triggers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using R3;


public class Elevator : EventTrigger
{
    [SerializeField] private List<float> floors;

    public ElevatorView ElevatorView;

    public ElevatorController ElevatorController = new();

    public List<float> Floors;
    public float CurrentFloors;

    [SerializeField] BoxCollider bxZero;

    private bool isIn;

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

        bxZero.OnTriggerEnterAsObservable()
            .Subscribe((collider) =>
            {
                if (isIn == false) { return; }
                if (collider.tag != "Player") { return; }

                isIn = false;
                GoFloor(9);
            }).AddTo(this);
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        other.transform.SetParent(gameObject.transform);

        isIn = true;
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        other.transform.SetParent(null);
    }

    public void HandleInteract(InputAction.CallbackContext _)
    {
        viewManager.LoadView<ElevatorView>(ElevatorController);
    }

    public void GoFloor(int f)  
    {
        float time = f;
        if (f == 9) 
        {
            transform.DOMoveY(0f * f, 10);
            return;
        }

        transform.DOMoveY(16.65f * f, 5 * time); 
    }

}