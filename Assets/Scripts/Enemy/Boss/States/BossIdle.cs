using System;
using UnityEngine;

public class BossIdle : IBossState
{
    public void UpdateState(BossStateMachine controller)
    {

    }

    public void OnEnter(BossStateMachine controller)
    {
        if (controller.bossController.enemyManager.player.isDead) return;
        switch (controller.attackType)
        {
            case AttackType.Melee:
                controller.ChangeState(controller.walkState);
                break;
            case AttackType.ProjectileRanged:
            case AttackType.Jump:
            case AttackType.Sing:
                Debug.LogWarning("Attack hasn't been coded yet.");
                break;
            case AttackType.Nymph:
                controller.ChangeState(controller.nymphIdleState);
                break;
            case AttackType.None:
            default:
                Debug.LogWarning("Enemy has no attack type. Please fix!!");
                break;
        }
    }

    public void OnExit(BossStateMachine controller)
    {

    }

    public void OnHurt(BossStateMachine controller)
    {

    }
}