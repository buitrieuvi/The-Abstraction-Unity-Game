using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAttackA1 : PlayerAttack
{

    public PlayerAttackA1(PlayerState playerState) : base(playerState)
    {
    }

    public override async void Enter()
    {
        //playerState.player.Animator.SetFloat(playerState.player.AnimationAttackParameterHash, 1.1f);
        //await UniTask.WaitForSeconds(0.5f);
        //playerState.ChangeState(playerState.player.Movement.idling);

        base.Enter();
    }

    public override void Exit()
    {
        //playerState.player.Animator.SetFloat(playerState.player.AnimationAttackParameterHash, 0f);
        base.Exit();
    }

    public override void HandleInput()
    {
        base.HandleInput();
    }

    public override void OntriggerEnter(Collider coll)
    {
        base.OntriggerEnter(coll);
    }

    public override void OntriggerExit(Collider coll)
    {
        base.OntriggerExit(coll);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void Update()
    {
        base.Update();
    }
}