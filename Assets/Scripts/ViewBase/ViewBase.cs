using DG.Tweening;
using R3;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;

public abstract class ViewBase : MonoBehaviour, IViewBase,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Inject] protected DataManager dataManager;
    [Inject] protected ViewManager viewManager;

    protected RectTransform tfView;

    public UnityAction<ViewBase> OnEnter;
    public UnityAction<ViewBase> OnExit;

    public UnityAction<ViewBase> OnClick;




    public virtual void Awake()
    {
        tfView = GetComponent<RectTransform>();
    }

    public virtual void Open(ControllerBase ctrl)
    {
        gameObject.SetActive(true);
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnter();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExit();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        PointerClick();
    }

    public virtual void PointerEnter()
    {
        if (OnEnter != null)
        {
            OnEnter.Invoke(this);
        }
    }
    public virtual void PointerExit()
    {
        if (OnExit != null)
        {
            OnExit.Invoke(this);
        }
    }
    public virtual void PointerClick()
    {
        OnClick?.Invoke(this);
    }
    public virtual void OnChanged() 
    {

    }

    public void Dotween_1() 
    {
        tfView.DOScale(1.1f, 0.35f).SetEase(Ease.OutBack).From(1);
    }

    public void Dotween_2()
    {
        tfView.DOScale(1f, 0.35f).SetEase(Ease.OutBack);
    }
}
