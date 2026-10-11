using DG.Tweening;

public class GameObjectMoveUp : GameObjectBase
{
    //[Inject] public LayerController Layer { get; private set; }

    public void Start()
    {
        OnEnter += (go) =>
        {
            go.transform.DOMoveY(10, 10f);
        };
    }
}
