using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using System;
using UnityEngine;

public class CubeIns : MonoBehaviour
{
    public GameObject Prefab;

    // Dùng Start thay vì Update
    async void Start()
    {
        Observable.Interval(TimeSpan.FromSeconds(0.35f))
        .Subscribe(_ =>
        {
            Instantiate(Prefab, transform);
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