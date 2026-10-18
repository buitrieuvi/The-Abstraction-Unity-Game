using DG.Tweening;

public class Ani_HoverEnter : ViewBase
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
        tfView.DOScale(1.1f, time).SetEase(Ease.OutBack);
    }
}
