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
                            var c = Instantiate(Prefab, transform);
                            c.transform.SetParent(null);

                            float x = UnityEngine.Random.Range(0.05f, 0.2f);
                            float y = UnityEngine.Random.Range(0.05f, 0.2f);
                            float z = UnityEngine.Random.Range(0.05f, 0.2f);
                            c.transform.localScale.Set(x, y, z);


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
                        if (Prefab != null) Instantiate(Prefab, transform);
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

    public override void TriggerHoverEnter()
    {
        base.TriggerHoverEnter();
    }

    public override void TriggerHoverExit()
    {
        base.TriggerHoverExit();
    }

    private void OnDestroy()
    {
        gear?.Kill();
    }
}