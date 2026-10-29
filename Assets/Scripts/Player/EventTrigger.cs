using DG.Tweening;
using EasyTextEffects;
using ITISKIRUHERE;
using TMPro;
using Unity.Pipeline;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public abstract class EventTrigger : MonoBehaviour,
    IEventTrigger
{
    //public GameObject Owner;

    public UnityAction OnTriggerEnterEvent;
    public UnityAction OnTriggerExitEvent;

    public UnityAction OnInteract;

    public UnityAction OnPick;
    public UnityAction OnThow;

    [Inject] protected InputManager inputManager;
    [Inject] protected ViewManager viewManager;
    [Inject] protected DataManager dataManager;

    public AdvancedOutline outline3d;

    public void TriggerEnter()
    {
        OnTriggerEnterEvent?.Invoke();
    }

    public void TriggerExit()
    {
        OnTriggerExitEvent?.Invoke();
    }


    public virtual void OnTriggerEnter(Collider other)
    {
        TriggerEnter();
    }

    public virtual void OnTriggerExit(Collider other)
    {
        TriggerExit();
    }


    public virtual void OnInteracted() 
    {
        OnInteract?.Invoke();
    }

    public virtual void OnPicked()
    {
        OnPick?.Invoke();
    }

    public virtual void OnThrowed()
    {
        OnThow?.Invoke();
    }

    public virtual void TriggerHoverEnter()
    {
        if (outline3d == null) return;
        outline3d.OutlineWidth = 10;
    }

    public virtual void TriggerHoverExit()
    {
        if (outline3d == null) return;
        outline3d.OutlineWidth = 0;
    }
}
