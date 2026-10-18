using DG.Tweening;

public class Ani_HoverEnter : ViewBase
{
    public float time = 0.35f;
    public void Start()
    {
        OnEnter += (e) => tfView.DOScale(1.1f, time).SetEase(Ease.OutBack).From(1);
    }
}
