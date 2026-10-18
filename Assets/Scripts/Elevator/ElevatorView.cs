using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElevatorView : ViewBase
{
    [SerializeField] private Transform tfBtn;
    List<ViewButtonBase> btns;

    public override void Awake()
    {
        base.Awake();
        btns = GetComponentsInChildren<ViewButtonBase>().ToList();
    }

    public override void Close()
    {
        base.Close();

        btns.Clear();

        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.Movement.ChangeState(playerController.Movement.idling);
        }
    }

    public override void OnChanged()
    {
        base.OnChanged();
    }

    public override void Open(ControllerBase ctrl)
    {
        base.Open(ctrl);
        ElevatorController elevatorController = ctrl as ElevatorController;

        foreach (var btn in btns)
        {
            btn.OnClick += (e) =>
            {
                int f = btn.Floors() + 1;
                if (f == 11) f = 0;

                elevatorController.GoFloor(f);
            };
        }

        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null) 
        {
            
            playerController.Movement.ChangeState(playerController.Movement.freezing);
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
