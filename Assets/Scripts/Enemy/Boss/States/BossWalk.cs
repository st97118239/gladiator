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

        if (controller.abilityType == BossAbility.Summon && distance <= controller.bossController.boss.abilityRadius)
        {
            if (controller.CanSummon())
            {
                controller.StartSummonCountdown();
                return;
            }
        }

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

        if (controller.canDash && controller.abilityDelay < 0 && distance >= controller.abilityRadius)
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
        if (controller.isFrozen || !controller.canDash || controller.abilityDelay >= 0) return;

        int shouldDash = Random.Range(0, 101);

        if (shouldDash <= controller.bossController.boss.abilityUseChance) 
            controller.Dash();
    }
}
