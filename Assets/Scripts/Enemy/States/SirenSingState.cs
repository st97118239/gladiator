using System;
using UnityEngine;

public class SirenSingState : IEnemyState
{
    private Transform playerTransform;
    private float singDrawSpeed;

    public void UpdateState(EnemyStateMachine controller)
    {
        if (controller.enemyController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        if (!controller.enemyController.isClosestSiren) return;

        if (Vector3.Distance(playerTransform.position, controller.transform.position) <= controller.enemyController.enemy.attackRadius)
        {
            controller.ChangeState(controller.attackState);
            return;
        }

        playerTransform.position = Vector3.MoveTowards(playerTransform.position, controller.transform.position, singDrawSpeed * Time.deltaTime);
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        playerTransform = controller.enemyController.enemyManager.player.transform;
        singDrawSpeed = controller.enemyController.enemy.singDrawSpeed;
    }

    public void OnExit(EnemyStateMachine controller)
    {
        
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
