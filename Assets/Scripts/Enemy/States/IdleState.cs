using System;
using UnityEngine;

public class IdleState : IEnemyState
{
    public void UpdateState(EnemyStateMachine controller)
    {
        
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        if (controller.enemyController.enemyManager.player.isDead) return;
        switch (controller.attackType)
        {
            case AttackType.Melee:
                controller.ChangeState(controller.walkState);
                break;
            case AttackType.ProjectileRanged:
                controller.ChangeState(controller.rangedWalkState);
                break;
            case AttackType.Jump:
                controller.ChangeState(controller.jumpWalkState);
                break;
            case AttackType.Sing:
                controller.ChangeState(controller.sirenSingState);
                break;
            case AttackType.MeleeBlock:
                controller.ChangeState(controller.walkState);
                break;
            case AttackType.Nymph:
                Debug.LogError("Enemy has a boss attack type. Please fix!");
                break;
            case AttackType.None:
            default:
                Debug.LogError("Enemy has no attack type. Please fix!!");
                break;
        }
    }

    public void OnExit(EnemyStateMachine controller)
    {
        
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
