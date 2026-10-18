using DG.Tweening;

public class Ani_HoverExit : ViewBase
{
    public float time = 0.35f;
    public void OnEnable()
    {
        OnExit += (e) => Ani();
    }

    public void OnDisable()
    {
        OnExit -= (e) => Ani();
    }

    public void Ani() 
    {
        tfView.DOScale(1f, time).SetEase(Ease.OutBack);
    }
}
