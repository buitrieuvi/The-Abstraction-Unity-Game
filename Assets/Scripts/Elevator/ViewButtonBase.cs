

using DG.Tweening;
using UnityEngine;

public class ViewButtonBase : ViewBase
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
    }

    public override void PointerExit()
    {
        base.PointerExit();
    }

    public int Floors() 
    {
        return transform.GetSiblingIndex();
    }

    public void Anim_MenuSlot() 
    {
        tfViewChild.DOMoveX(250f, 0.35f).From(-500).SetEase(Ease.OutBack).SetDelay(tfView.transform.GetSiblingIndex() * 0.02f);
    }
}
