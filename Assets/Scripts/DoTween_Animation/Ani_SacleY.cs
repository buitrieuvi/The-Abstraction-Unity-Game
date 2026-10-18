using DG.Tweening;
using UnityEngine;

public class Ani_SacleY : ViewBase
{
    public float time = 0.35f;
    public void Start()
    {
        OnEnter += (e) => tfView.DOScaleY(1, time).From(0);
    }
}
