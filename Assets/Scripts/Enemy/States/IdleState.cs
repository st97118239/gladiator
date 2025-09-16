using System;
using UnityEngine;

public class IdleState : IEnemyState
{
    public void UpdateState(EnemyStateMachine controller)
    {
        
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        if (!controller.enemyController.enemyManager.player.isDead)
        {
            switch (controller.enemyController.enemy.attackType)
            {
                case AttackType.Melee:
                    controller.ChangeState(controller.walkState);
                    break;
                case AttackType.ProjectileRanged:
                    controller.ChangeState(controller.rangedWalkState);
                    break;
                case AttackType.Jump:
                case AttackType.Sing:
                    break;
                case AttackType.None:
                default:
                    Debug.LogWarning("Enemy has no attack type. Please fix!!");
                    break;
            }
        }
        else
        {
            Debug.Log("Player is fucking dead;");
        }
    }

    public void OnExit(EnemyStateMachine controller)
    {
        
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
