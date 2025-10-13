using UnityEngine;

public class ZeusWalk : IBossState
{
    private Player player;
    private float speed;
    private int posIdx;
    private Vector3 posToGoTo;
    private bool reachedPos;

    // TODO: Zeus has to walk to random ranged position, then if player is near, make them walk to another position where player is *not* near

    public void UpdateState(BossStateMachine controller)
    {
        if (controller.bossController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        float playerDistance = Vector3.Distance(controller.gameObject.transform.position, player.transform.position);

        if (!controller.isReloading && controller.abilityType == BossAbility.Thunder && controller.abilityDelay < 0 && playerDistance <= controller.bossController.boss.abilityRadius)
        {
            int swipeChance = Random.Range(0, 101);

            if (swipeChance <= controller.bossController.boss.abilityUseChance)
            {
                controller.ChangeState(controller.zeusLightningState);
                return;
            }
        }

        if (!controller.isReloading && playerDistance <= controller.bossController.boss.attackRadius && playerDistance > controller.bossController.boss.rangedFleeRadius)
        {
            controller.ChangeState(controller.zeusAttackState);
            return;
        }

        switch (reachedPos)
        {
            case false when controller.gameObject.transform.position == posToGoTo:
                reachedPos = true;
                break;
            case true when playerDistance <= controller.bossController.boss.rangedFleeRadius:
                OnEnter(controller);
                return;
        }

        controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, posToGoTo, speed * Time.deltaTime);

        if (player.transform.position.x < controller.transform.position.x)
            controller.bossController.spriteRenderer.flipX = true;
        else if (player.transform.position.x > controller.transform.position.x)
            controller.bossController.spriteRenderer.flipX = false;
    }

    public void OnEnter(BossStateMachine controller)
    {
        reachedPos = false;
        player = controller.bossController.enemyManager.player;
        speed = controller.bossController.boss.speed;
        posIdx = Random.Range(0, controller.bossController.enemyManager.rangedPositions.Count);
        posToGoTo = controller.bossController.enemyManager.rangedPositions[posIdx].position;
    }

    public void OnExit(BossStateMachine controller)
    {

    }

    public void OnHurt(BossStateMachine controller)
    {

    }
}
