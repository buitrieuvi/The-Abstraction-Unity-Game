using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System;
using UnityEngine;

public class MachineSpawnCube : EventTrigger
{
    public GameObject Prefab;
    public GameObject Gear;
    public float time = 100f;
    public bool AutoSpawm;


    private Tween gear;

    public override void OnInteracted()
    {
        base.OnInteracted();
    }

    public override void OnPicked()
    {
        base.OnPicked();


        if (gear == null)
        {
            if (AutoSpawm)
            {
                gear = Gear.transform.DOLocalRotate(new Vector3(0, 0, 360), time, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart)
                    .SetAutoKill(false)
                    .OnStepComplete(() =>
                    {
                        if (Prefab != null) Instantiate(Prefab, transform);
                    });
            }
            else
            {
                gear = Gear.transform.DOLocalRotate(new Vector3(0, 0, 360), time, RotateMode.FastBeyond360)
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