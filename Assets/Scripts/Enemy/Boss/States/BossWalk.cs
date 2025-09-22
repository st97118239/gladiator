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
        
        if (controller.abilityType == BossAbility.Dash && distance >= controller.bossController.boss.abilityRadius)
        {
            controller.ChangeState(controller.dashState);
            return;
        }

        controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, player.transform.position, speed * Time.deltaTime);
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
