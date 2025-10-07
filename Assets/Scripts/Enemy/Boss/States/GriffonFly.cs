using UnityEngine;

public class GriffonFly : IBossState
{
    private Player player;
    private float speed;
    private bool willSwipeAttack;
    private Vector3 swipePointPos;

    public void UpdateState(BossStateMachine controller)
    {
        if (controller.bossController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        float distance = 0;

        if (willSwipeAttack)
        {
            if (controller.currentPlatform != player.currentPlatform)
            {
                willSwipeAttack = false;
                swipePointPos = Vector3.zero;
                return;
            }

            distance = Vector3.Distance(controller.transform.position, swipePointPos);

            if (distance <= controller.bossController.boss.extraAttackRadius)
            {
                controller.ChangeState(controller.griffonSwipeState);
                willSwipeAttack = false;
                swipePointPos = Vector3.zero;
                return;
            }

            controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, swipePointPos, speed * Time.deltaTime);

            return;
        }

        if (controller.isOnPlatform && controller.currentPlatform == player.currentPlatform)
        {
            int swipeChance = Random.Range(0, 101);

            if (swipeChance <= controller.bossController.boss.swipeAttackChance)
            {
                willSwipeAttack = true;
                swipePointPos = controller.bossController.enemyManager.GetPlatformSwipePos(controller.transform.position);
                return;
            }
        }

        distance = Vector3.Distance(controller.transform.position, player.transform.position);

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
        willSwipeAttack = false;
        swipePointPos = Vector3.zero;
    }

    public void OnExit(BossStateMachine controller)
    {

    }

    public void OnHurt(BossStateMachine controller)
    {
        if (!controller.canDash || controller.abilityDelay >= 0) return;

        int shouldDash = Random.Range(0, 101);

        if (shouldDash <= controller.bossController.boss.abilityUseChance) 
            controller.Dash();
    }
}
