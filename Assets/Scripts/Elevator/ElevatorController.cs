
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
    public List<Transform> FloorsTransform;
    public ElevatorView ElevatorView;
    public Elevator Elevator;

    public List<Transform> Door_floors;

    public int Gf;

    private int curr = 0;
    private int next;
    private Tween moveTween;

    public int numFloor = 0;
    private bool isPlay = false;

    public async void GoFloor(int f)
    {
        if (isPlay) return;

        if (Elevator == null)
        {
            Debug.LogError("Elevator chưa được gán!");
            return;
        }

        if (FloorsTransform == null || FloorsTransform.Count == 0)
        {
            Debug.LogError("FloorsTransform chưa được thiết lập!");
            return;
        }

        // Nếu chọn tầng hiện tại Gf thì quay về tầng 0
        //if (f == Gf)
        //    f = 0;

        // Kiểm tra chỉ số tầng
        if (f < 0 || f >= FloorsTransform.Count)
        {
            Debug.LogWarning($"Tầng {f} không tồn tại!");
            return;
        }

        if (FloorsTransform[f] == null)
        {
            Debug.LogError($"FloorsTransform[{f}] chưa được gán!");
            return;
        }

        next = f;

        // Tính thời gian di chuyển dựa trên số tầng
        int t = Mathf.Max(1, Mathf.Abs(next - curr));

        //if (moveTween.IsPlaying()) { return; }
        // Hủy chuyển động cũ nếu đang di chuyển
        moveTween?.Kill();
        isPlay = true;

        Elevator.Door(false, f);
        numFloor = f;
        moveTween = Elevator.transform
            .DOMoveY(FloorsTransform[f].position.y + 0.35f, 5f * t)
            .SetEase(Ease.InOutSine)
            .OnComplete(async () =>
            {
                isPlay = false;
                curr = next;
                Gf = curr;

                if (Elevator == null) { return; }
                if (Elevator.isIn) 
                {
                    await UniTask.WaitForSeconds(0.5f);
                    Elevator.Door(true, f);
                }

            }).SetDelay(2.5f);
    }
}
