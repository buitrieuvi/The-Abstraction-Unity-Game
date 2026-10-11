using UnityEngine;

public interface IState
{
    public void Enter();
    public void Update();
    public void PhysicsUpdate();
    public void HandleInput();
    public void Exit();

    public void OntriggerEnter(Collider coll);
    public void OntriggerExit(Collider coll);
}
