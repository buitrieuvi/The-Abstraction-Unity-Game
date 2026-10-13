using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorView : ViewBase
{
    [SerializeField] private List<ElevatorButtonView> btns;

    public Elevator Elevator;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Close()
    {
        base.Close();

        Elevator = null;
    }

    public override void OnChanged()
    {
        base.OnChanged();
    }

    public override void Open(ControllerBase ctrl)
    {
        base.Open(ctrl);

        ElevatorController elevatorController = ctrl as ElevatorController;
        Elevator = elevatorController.Elevator;

        foreach (var item in btns)
        {
            item.OnClick += (e) => 
            {
                Debug.Log("Go");
                Elevator.GoFloor(item.Floors() + 1);
            };
        }
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
