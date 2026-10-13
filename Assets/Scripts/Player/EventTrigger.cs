using UnityEngine;
using UnityEngine.Events;
using Zenject;

public abstract class EventTrigger : MonoBehaviour,
    IEventTrigger
{
    //public GameObject Owner;

    public UnityAction OnTriggerEnterEvent;
    public UnityAction OnTriggerExitEvent;

    [Inject] protected InputManager inputManager;
    [Inject] protected ViewManager viewManager;

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
}
