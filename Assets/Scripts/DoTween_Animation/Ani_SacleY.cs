using DG.Tweening;
using UnityEngine;

public class Ani_SacleY : ViewBase
{
    public float time = 0.35f;
    public void OnEnable()
    {
        OnEnter += (e) => Ani();
    }

    public void OnDisable()
    {
        OnEnter -= (e) => Ani();
    }

    public void Ani() 
    {
        tfView.DOScaleY(1, time).From(0);
    }
}
