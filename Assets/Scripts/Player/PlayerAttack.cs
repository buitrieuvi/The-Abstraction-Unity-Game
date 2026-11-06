using UnityEngine;

public class PlayerAttack : IState
{
    protected readonly PlayerState playerState;

    public PlayerAttack(PlayerState playerState)
    {
        this.playerState = playerState;
    }

    public virtual void Enter()
    {
        
    }

    public virtual void Exit()
    {
        
    }

    public virtual void HandleInput()
    {
        
    }

    public virtual void OntriggerEnter(Collider coll)
    {
        
    }

    public virtual void OntriggerExit(Collider coll)
    {
        
    }

    public virtual void PhysicsUpdate()
    {
        
    }

    public virtual void Update()
    {
        
    }
}
