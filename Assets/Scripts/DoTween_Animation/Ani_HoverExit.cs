using DG.Tweening;

public class Ani_HoverExit : ViewBase
{
    public float time = 0.35f;
    public void Start()
    {
        OnExit += (e) => tfView.DOScale(1f, time).SetEase(Ease.OutBack);
    }
}
