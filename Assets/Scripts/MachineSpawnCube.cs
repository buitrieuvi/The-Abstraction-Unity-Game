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

    void Start()
    {
        //// 1. Start the infinite rotation
        //Gear.transform.DOLocalRotate(new Vector3(0, 0, 360), time, RotateMode.FastBeyond360)
        //    .SetEase(Ease.Linear)
        //    .SetLoops(-1, LoopType.Restart);

        //// 2. Use R3 to trigger the spawn precisely every 'time' seconds
        //Observable.Interval(TimeSpan.FromSeconds(time))
        //    .Subscribe(_ =>
        //    {
        //        if (Prefab != null)
        //        {
        //            Instantiate(Prefab, transform);
        //        }
        //    })
        //    .AddTo(this);
    }



    public override void OnInteracted()
    {
        base.OnInteracted();
        Gear.transform.DOLocalRotate(new Vector3(0, 0, 360), time, RotateMode.FastBeyond360)
        .SetEase(Ease.Linear).OnComplete(() =>
        {
            Instantiate(Prefab, transform);
        });
    }
}