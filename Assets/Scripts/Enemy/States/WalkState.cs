using UnityEngine;

public class WalkState : IEnemyState
{
    private Player player;
    private float speed;

    public void UpdateState(EnemyStateMachine controller)
    {
        if (controller.enemyController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        if (Vector3.Distance(controller.transform.position, player.transform.position) <= controller.enemyController.enemy.attackRadius)
        {
            controller.ChangeState(controller.attackState);
            return;
        }

        controller.transform.position = Vector3.MoveTowards(controller.transform.position, player.transform.position, speed * Time.deltaTime);

        if (player.transform.position.x < controller.transform.position.x)
            controller.enemyController.spriteRenderer.flipX = true;
        else if (player.transform.position.x > controller.transform.position.x) 
            controller.enemyController.spriteRenderer.flipX = false;
    }
    
    public void OnEnter(EnemyStateMachine controller)
    {
        player = controller.enemyController.enemyManager.player;
        speed = controller.enemyController.enemy.speed;
    }

    public void OnExit(EnemyStateMachine controller)
    {

    }

    public void OnHurt(EnemyStateMachine controller)
    {
        if (controller.attackType == AttackType.MeleeBlock && controller.canBlock)
        {
            controller.ChangeState(controller.blockState);
        }
    }
}
