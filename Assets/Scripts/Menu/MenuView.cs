using UnityEngine;
using Unity;
using System;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;

public class MenuView : ViewBase
{
    [SerializeField] Transform tfMenu;
    List<ViewButtonBase> slots;

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

        slots = tfMenu.GetComponentsInChildren<ViewButtonBase>().ToList();
        foreach (var item in slots) 
        {
            item.transform.DOLocalMoveX(-1000, 0);
        }

        foreach (var item in slots)
        {
            item.OnEnter += (e) =>
            {
                item.Dotween_Ani_HoverEnter(1.05f);
            };

            item.OnExit += (e) =>
            {
                item.Dotween_Ani_HoverExit();
            };

            item.Dotween_Ani_MoveX(0.45f, 0.035f, -1000, 138f);
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
