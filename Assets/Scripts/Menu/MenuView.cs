using UnityEngine;
using Unity;
using System;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using Zenject;

public class MenuView : ViewBase
{
    [SerializeField] Transform tfMenu;

    [SerializeField] ViewButtonBase btnExitGame;
    List<ViewButtonBase> slots;


    [SerializeField] ViewSliderBase sldX;
    [SerializeField] ViewSliderBase sldY;


    public override void Awake()
    {
        base.Awake();


    }

    public void Start()
    {
        CameraManager cams = FindFirstObjectByType<CameraManager>();

        sldX.Slider.value = cams.CamX;
        sldY.Slider.value = cams.CamY;

        sldX.Slider.onValueChanged.AddListener((e) =>
        {
            cams.SetSpeedCamera(e, cams.CamY);
        });
        sldY.Slider.onValueChanged.AddListener((e) =>
        {
            cams.SetSpeedCamera(cams.CamX, e);
        });
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

        btnExitGame.OnClick += (e) => 
        {
            Application.Quit();
        };

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
