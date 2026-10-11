using UnityEngine;
using UnityEngine.Events;
using Zenject;

public abstract class GameObjectBase : MonoBehaviour, IGameObjectBase
{
    public UnityAction<GameObjectBase> OnEnter;

    public virtual void Enter(ControllerBase ctrl)
    {
        
    }

    public virtual void Exit()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnEnter?.Invoke(this);
        }
    }
}
