using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System;
using UnityEngine;

public class MachineSpawnCube : MonoBehaviour
{
    public GameObject Prefab;

    // Dùng Start thay vì Update

    public float time = 5;

    async void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(time))
        .Subscribe(_ =>
        {
            var cube = Instantiate(Prefab, transform);
        })
        .AddTo(this);

        //Observable.EveryUpdate()
        //.ThrottleFirst(TimeSpan.FromSeconds(1))
        //.Subscribe(_ =>
        //{
        //    Instantiate(Prefab, transform);
        //})
        //.AddTo(this);

    }


}