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
    public UnityAction TriggerEnterEvent;
    public UnityAction TriggerExitEvent;

    public UnityAction TriggerHoverEnterEvent;
    public UnityAction TriggerHoverExitEvent;

    public UnityAction Interact;

    public UnityAction Pick;
    public UnityAction Thorw;

    [Inject] protected InputManager inputManager;
    [Inject] protected ViewManager viewManager;
    [Inject] protected DataManager dataManager;

    public AdvancedOutline Outline3d;

    public void OnTriggerEnter()
    {
        TriggerEnterEvent?.Invoke();
    }

    public void OnTriggerExit()
    {
        TriggerExitEvent?.Invoke();
    }


    public virtual void OnTriggerEnter(Collider other)
    {
        OnTriggerEnter();
    }

    public virtual void OnTriggerExit(Collider other)
    {
        OnTriggerExit();
    }


    public virtual void OnInteracted() 
    {
        Interact?.Invoke();
    }

    public virtual void OnPicked()
    {
        Pick?.Invoke();
    }

    public virtual void OnThrowed()
    {
        Thorw?.Invoke();
    }

    public virtual void OnTriggerHoverEnter()
    {
        TriggerHoverEnterEvent?.Invoke();

        if (Outline3d != null)
        {
            Outline3d.OutlineWidth = 10;
        }
    }

    public virtual void OnTriggerHoverExit()
    {
        TriggerHoverExitEvent?.Invoke();

        if (Outline3d != null)
        {
            Outline3d.OutlineWidth = 0;
        }
    }
}
