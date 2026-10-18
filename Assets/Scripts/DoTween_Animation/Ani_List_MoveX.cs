using DG.Tweening;

public class Ani_List_MoveX : ViewBase
{
    public float time = 0.35f;
    
    public float X = 0;
    public float Y = 500;

    public float Space = 0.002f;

    public override void Awake()
    {
        base.Awake();   
    }


    public void OnEnable()
    {
        int index = gameObject.transform.GetSiblingIndex();
        tfView.DOMoveX(Y, time).From(X).SetDelay(index* Space);
    }
}