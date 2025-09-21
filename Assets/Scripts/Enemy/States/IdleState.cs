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
                Debug.Log("Attack hasn't been coded yet.");
                break;
            case AttackType.Sing:
                controller.ChangeState(controller.sirenSingState);
                break;
            case AttackType.None:
            default:
                Debug.LogWarning("Enemy has no attack type. Please fix!!");
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
