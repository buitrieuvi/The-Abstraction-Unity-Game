using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class MachineSpawnCubeController : ControllerBase 
{
    public float Time = 10f;
    public bool AutoSpawm = false;

}

public class MachineSpawnCube : EventTrigger
{
    public GameObject Prefab;
    public GameObject Gear;

    public Transform farm;

    public MachineSpawnCubeController controller = new();

    private Tween gear;

    public ViewUpgrade viewUpgrade;

    public override async void OnInteracted()
    {
        base.OnInteracted();

        viewUpgrade = (ViewUpgrade) await viewManager.ShowView<ViewUpgrade>(controller);
    }

    public override void OnPicked()
    {
        base.OnPicked();


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
                            var c = Instantiate(Prefab, farm);
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
                        if (Prefab != null) Instantiate(Prefab, farm);
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