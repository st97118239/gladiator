using UnityEngine;

public class BossWalk : IBossState
{
    private Player player;
    private float speed;

    public void UpdateState(BossStateMachine controller)
    {
        if (controller.bossController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        float distance = Vector3.Distance(controller.gameObject.transform.position, player.transform.position);

        if (distance <= controller.bossController.boss.attackRadius)
        {
            controller.ChangeState(controller.meleeState);
            return;
        }

        controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, player.transform.position, speed * Time.deltaTime);

        if (player.transform.position.x < controller.transform.position.x)
            controller.bossController.spriteRenderer.flipX = true;
        else if (player.transform.position.x > controller.transform.position.x)
            controller.bossController.spriteRenderer.flipX = false;

        if (controller.canDash && controller.dashDelay < 0)
            controller.Dash();
    }
    
    public void OnEnter(BossStateMachine controller)
    {
        player = controller.bossController.enemyManager.player;
        speed = controller.bossController.boss.speed;
    }

    public void OnExit(BossStateMachine controller)
    {

    }

    public void OnHurt(BossStateMachine controller)
    {

    }
}
