using UnityEngine;

public class WalkState : IEnemyState
{
    private Transform playerTransform;
    private float speed;

    public void UpdateState(EnemyStateMachine controller)
    {
        if (controller.enemyController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        if (Vector3.Distance(controller.transform.position, playerTransform.position) <= controller.enemyController.enemy.attackRadius)
        {
            controller.ChangeState(controller.attackState);
            return;
        }

        controller.transform.position = Vector3.MoveTowards(controller.transform.position, playerTransform.position, speed * Time.deltaTime);

        if (playerTransform.position.x < controller.transform.position.x)
            controller.enemyController.spriteRenderer.flipX = true;
        else if (playerTransform.position.x > controller.transform.position.x) 
            controller.enemyController.spriteRenderer.flipX = false;

        if (controller.canDash && controller.dashDelay < 0)
            controller.Dash();
    }
    
    public void OnEnter(EnemyStateMachine controller)
    {
        playerTransform = controller.enemyController.enemyManager.player.transform;
        speed = controller.enemyController.enemy.speed;
        controller.enemyController.animator.SetBool("Walk", true);
    }

    public void OnExit(EnemyStateMachine controller)
    {
        controller.enemyController.animator.SetBool("Walk", false);
        controller.spriteRenderer.sprite = controller.enemyController.enemy.sprite;
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        if (controller.attackType == AttackType.MeleeBlock && controller.canBlock)
        {
            controller.ChangeState(controller.blockState);
        }
    }
}
