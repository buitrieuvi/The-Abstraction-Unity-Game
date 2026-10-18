using DG.Tweening;

public class Ani_List_Scale : ViewBase
{
    public float time = 0.35f;

    public float Space = 0.02f;

    public override void Awake()
    {
        base.Awake();
    }


    public void Start()
    {
        int index = gameObject.transform.GetSiblingIndex();
        tfView.DOScale(1, time).From(0).SetDelay(index * Space);
    }
}