using System;
using UnityEngine;

public abstract class StateMachine
{
    protected IState currentState;
    public IState CurrentState => currentState;

    public void ChangeState(IState newState)
    {
        if (newState == null)
        {
            throw new ArgumentNullException(nameof(newState));
        }

        if (ReferenceEquals(currentState, newState))
        {
            return;
        }

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void HandleInput() => currentState?.HandleInput();
    public void Update() => currentState?.Update();
    public void PhysicsUpdate() => currentState?.PhysicsUpdate();
    public void OntriggerEnter(Collider collider) => currentState?.OntriggerEnter(collider);
    public void OntriggerExit(Collider collider) => currentState?.OntriggerExit(collider);
}
