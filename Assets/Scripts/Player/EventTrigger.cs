using DG.Tweening;
using EasyTextEffects;
using TMPro;
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

    [Inject] protected InputManager inputManager;
    [Inject] protected ViewManager viewManager;
    [Inject] protected DataManager dataManager;

    [SerializeField] TextMeshProUGUI text;
    [SerializeField] TextEffect textEf;

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

        //SetText();
    }

    public virtual void OnTriggerExit(Collider other)
    {
        TriggerExit();
    }

    public void SetText() 
    {
        textEf.Refresh();
    }

    public virtual void OnOnInteracted() 
    {
        OnInteract?.Invoke();
    }

    public void Ani_UpDown() 
    {
        //transform.DOMove(transform.position + new Vector3(0, 1, 0), 2).Rewind();
    }
    public void Ani_ScaleZero()
    {

    }
}
