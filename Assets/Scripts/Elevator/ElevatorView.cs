using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorView : ViewBase
{
    [SerializeField] private List<ElevatorButtonView> btns;

    public Transport_Elevator Elevator;

    public override void Awake()
    {
        base.Awake();

        btns[0].OnClick += (e) =>
        {
            Elevator.GoFloor();
        };
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
}
