using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCategoryView : ViewBase
{
    public string CategoryName;
    public Outline Outline;

    public override void Awake()
    {
        base.Awake();
        Outline = GetComponent<Outline>();
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

    public void OnOutline(bool isOn)
    {
        if (Outline != null)
        {
            Outline.enabled = isOn;
        }
    }
}
