using DG.Tweening;
using UnityEngine;

public class TagView : ViewBase
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void Close()
    {
        base.Close();
    }

    public override void OnChanged()
    {
        base.OnChanged();
    }

    public override void Open(ControllerBase ctrl) 
    {
        base.Open(ctrl);
    }

    public override void PointerClick()
    {
        base.PointerClick();
    }

    public override void PointerEnter()
    {
        base.PointerEnter();
        tfView.DOScale(1.1f, 0.2f).SetEase(Ease.OutBack).From(1);
    }

    public override void PointerExit()
    {
        base.PointerExit();
        tfView.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
    }
}
