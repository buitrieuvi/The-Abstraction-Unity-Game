
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

[Serializable]
public class ElevatorController : ControllerBase
{
    public Door Door;
    public ElevatorView ElevatorView;
    public Elevator Elevator;

    public List<Transform> FloorsTransform;
    public List<Transform> Door_floors;

    public int Gf;

    public int Curr = 0;
    public int Next;
    private Tween moveTween;

    public int NumFloor = 0;
    public bool IsPlay = false;

    public async void GoFloor(int f)
    {
        //if (IsPlay) return;

        //if (Elevator == null)
        //{
        //    Debug.LogError("Elevator chưa được gán!");
        //    return;
        //}

        //if (FloorsTransform == null || FloorsTransform.Count == 0)
        //{
        //    Debug.LogError("FloorsTransform chưa được thiết lập!");
        //    return;
        //}

        //// Nếu chọn tầng hiện tại Gf thì quay về tầng 0
        ////if (f == Gf)
        ////    f = 0;

        //// Kiểm tra chỉ số tầng
        //if (f < 0 || f >= FloorsTransform.Count)
        //{
        //    Debug.LogWarning($"Tầng {f} không tồn tại!");
        //    return;
        //}

        //if (FloorsTransform[f] == null)
        //{
        //    Debug.LogError($"FloorsTransform[{f}] chưa được gán!");
        //    return;
        //}

        //Next = f;

        //// Tính thời gian di chuyển dựa trên số tầng
        //int t = Mathf.Max(1, Mathf.Abs(Next - Curr));

        ////if (moveTween.IsPlaying()) { return; }
        //// Hủy chuyển động cũ nếu đang di chuyển
        //moveTween?.Kill();
        //IsPlay = true;

        //Elevator.Door(false, f);
        //NumFloor = f;
        //moveTween = Elevator.transform
        //    .DOMoveY(FloorsTransform[f].position.y + 0.35f, 5f * t)
        //    .SetEase(Ease.InOutSine)
        //    .OnComplete(async () =>
        //    {
        //        IsPlay = false;
        //        Curr = Next;
        //        Gf = Curr;

        //        if (Elevator == null) { return; }
        //        if (Elevator.isIn) 
        //        {
        //            await UniTask.WaitForSeconds(0.5f);
        //            Elevator.Door(true, f);
        //        }

        //    }).SetDelay(2.5f);
    }
}
