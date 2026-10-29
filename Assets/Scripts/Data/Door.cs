using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3.Triggers;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : EventTrigger
{
    [SerializeField] GameObject l;
    [SerializeField] GameObject r;

    [SerializeField] float o;
    [SerializeField] float c;

    private void Start()
    {

    }

    public void OnOpen(bool b) 
    {
        l.transform?.DOKill();
        r.transform?.DOKill();

        if (b)
        {
            l.transform.DOMoveX(o, 2);
            r.transform.DOMoveX(-o, 2);
        }

        else 
        {
            l.transform.DOMoveX(c, 2);
            r.transform.DOMoveX(-c, 2);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        inputManager.InputActions.Player.Interact.started += HandleInteract;
    }

    public override async void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);


        inputManager.InputActions.Player.Interact.started -= HandleInteract;
    }

    public async void HandleInteract(InputAction.CallbackContext _)
    {
        OnInteracted();
    }

    public override void OnInteracted()
    {
        base.OnInteracted();

    }
}
