using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Transport_Elevator : EventTrigger
{
    [SerializeField] private List<float> floors;

    public void Awake()
    {
        OnTriggerEnterEvent += () =>
        {
            //transform.DOMoveY(16.65f, 5f);
            Debug.Log("Open");
            inputManager.InputActions.Player.Interact.started += HandleInteract;
        };

        OnTriggerExitEvent += () => 
        {
            Debug.Log("Close");
            inputManager.InputActions.Player.Interact.started -= HandleInteract;
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

    private void HandleInteract(InputAction.CallbackContext _)
    {
        viewManager.LoadView<ElevatorView>(null);
    }


}