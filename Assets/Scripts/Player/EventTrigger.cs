using UnityEngine;

public abstract class EventTrigger : MonoBehaviour,
    IEventTrigger
{
    public virtual void TriggerEnter()
    {

    }

    public virtual void TriggerExit()
    {

    }

    public virtual void OnInteract()
    {

    }
}
