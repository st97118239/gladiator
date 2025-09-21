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

        if (Vector3.Distance(controller.gameObject.transform.position, player.transform.position) <= controller.enemyController.enemy.attackRadius)
        {
            controller.ChangeState(controller.attackState);
            return;
        }

        controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, player.transform.position, speed * Time.deltaTime);
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

    }
}
