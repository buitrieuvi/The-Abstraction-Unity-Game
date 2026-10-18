using UnityEngine;
using Unity;
using System;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;

public class MenuView : ViewBase
{
    [SerializeField] Transform tfMenu;
        
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

        //List<ViewButtonBase> slots = tfMenu.GetComponentsInChildren<ViewButtonBase>().ToList();

        //foreach (var item in slots)
        //{
        //    item.Anim_MenuSlot();

        //    item.OnEnter += (e) =>
        //    {
        //        item.Dotween_1();
        //    };

        //    item.OnExit += (e) =>
        //    {
        //        item.Dotween_2();
        //    };
        //}
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
