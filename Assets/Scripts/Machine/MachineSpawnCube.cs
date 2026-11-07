using DG.Tweening;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using System;
using UnityEngine;

[Serializable]
public class MachineSpawnCubeController : ControllerBase 
{
    public float Time = 2f;
    public bool AutoSpawm = false;

    public ItemSOFactory[] ItemSOFactorys;
}

public class MachineSpawnCube : EventTrigger
{
    public GameObject Prefab;
    public GameObject Gear;

    public Transform farm;

    public MachineSpawnCubeController controller = new();

    private Tween gear;

    public ViewUpgrade viewUpgrade;

    public void Start()
    {
        controller.ItemSOFactorys.ForEach(itemSOFactory =>
        {
            itemSOFactory.Run(controller);
        });
    }

    public override async void OnInteracted()
    {
        base.OnInteracted();

        viewUpgrade = (ViewUpgrade) await viewManager.ShowView<ViewUpgrade>(controller);
    }

    public override void OnPicked()
    {
        base.OnPicked();

        GameObject cube = new GameObject();

        if (gear == null)
        {
            if (controller.AutoSpawm)
            {
                gear = Gear.transform.DOLocalRotate(new Vector3(0, 0, 360), controller.Time, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart)
                    .SetAutoKill(false) 
                    .OnStepComplete(() =>
                    {
                        if (Prefab != null) 
                        {
                            cube = Instantiate(Prefab, farm);
                        }
                    });
            }
            else
            {
                gear = Gear.transform.DOLocalRotate(new Vector3(0, 0, 360), controller.Time, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetAutoKill(false)
                    .OnComplete(() =>
                    {
                        if (Prefab != null) 
                        {
                            cube = Instantiate(Prefab, farm);
                        }
                    });
            }

            gear.Play();
        }
        else
        {
            if (gear.IsPlaying()) { return; }
            gear.Restart();
        }
    }

    public override void OnTriggerHoverEnter()
    {
        base.OnTriggerHoverEnter();
    }

    public override void OnTriggerHoverExit()
    {
        base.OnTriggerHoverExit();
    }

    private void OnDestroy()
    {
        gear?.Kill();
    }
}