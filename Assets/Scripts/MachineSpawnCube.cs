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
    public bool autoSpawm = false;

    public GameObject View;

    private Tween gear;

    public override void OnInteracted()
    {
        base.OnInteracted();

        View.gameObject.SetActive(true);
    }

    public override void OnPicked()
    {
        base.OnPicked();


        if (gear == null)
        {
            if (autoSpawm)
            {
                // Continuous spinning with spawning on every loop completion
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
                // Single rotation spawning once on complete
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

    private void OnDestroy()
    {
        // Always good practice to kill DOTween tweens when the object is destroyed
        gear?.Kill();
    }
}