
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ElevatorController : ControllerBase
{
    public List<Transform> FloorsTransform;
    public ElevatorView ElevatorView;
    public Elevator Elevator;

    public int Gf;

    private int curr = 0;
    private int next;
    private Tween moveTween;

    private bool isPlay = false;

    public void GoFloor(int f)
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
        moveTween = Elevator.transform
            .DOMoveY(FloorsTransform[f].position.y + 0.35f, 5f * t)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                isPlay = false;
                curr = next;
                Gf = curr;
            });
    }
}
