using System.Linq;
using UnityEngine;

public class ZeusWalk : IBossState
{
    private Player player;
    private float speed;
    private bool reachedPos;
    private bool goToRangedPoint;
    private bool isAttacking;

    private RangedPoint pointToGoTo;
    private Vector3 posToRunTo;

    public void UpdateState(BossStateMachine controller)
    {
        if (controller.bossController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        float playerDistance = Vector3.Distance(controller.transform.position, player.transform.position);
        float posDistance = Vector3.Distance(controller.transform.position, posToRunTo);

        if (reachedPos)
        {
            if (!controller.isReloading && controller.abilityType == BossAbility.Thunder && controller.abilityDelay < 0 && playerDistance <= controller.bossController.boss.abilityRadius)
            {
                int swipeChance = Random.Range(0, 101);

                if (swipeChance <= controller.bossController.boss.abilityUseChance)
                {
                    controller.ChangeState(controller.zeusLightningState);
                    return;
                }
            }

            if (playerDistance <= controller.bossController.boss.rangedFleeRadius)
            {
                reachedPos = false;
                isAttacking = false;
                goToRangedPoint = true;
                CalculateNextPath(controller);
                return;
            }

            if (playerDistance <= controller.bossController.boss.attackRadius && !controller.isReloading)
            {
                isAttacking = true;
                controller.ChangeState(controller.zeusAttackState);
                return;
            }

            isAttacking = false;
        }
        else if (posDistance <= 0.2f)
        {
            if (goToRangedPoint)
            {
                reachedPos = true;
                goToRangedPoint = false;
            }
        }

        if (!reachedPos)
            controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, posToRunTo, speed * Time.deltaTime);

        if (player.transform.position.x < controller.transform.position.x)
            controller.bossController.spriteRenderer.flipX = true;
        else if (player.transform.position.x > controller.transform.position.x)
            controller.bossController.spriteRenderer.flipX = false;
    }

    public void OnEnter(BossStateMachine controller)
    {
        if (isAttacking)
        {
            float playerDistance = Vector3.Distance(controller.transform.position, player.transform.position);
            if (playerDistance <= controller.bossController.boss.attackRadius && playerDistance > controller.bossController.boss.rangedFleeRadius)
            {
                controller.ChangeState(controller.zeusAttackState);
                isAttacking = true;
                return;
            }
        }

        isAttacking = false;
        reachedPos = false;
        player = controller.bossController.enemyManager.player;
        speed = controller.bossController.boss.speed;
        goToRangedPoint = true;
        CalculateNextPath(controller);
    }

    public void OnExit(BossStateMachine controller)
    {

    }

    public void OnHurt(BossStateMachine controller)
    {

    }

    private void CalculateNextPath(BossStateMachine controller)
    {
        if (!goToRangedPoint) return;

        RangedPoint[] closestPoints = new RangedPoint[controller.bossController.boss.rangedPointsToCheck];
        int idx = 0;
        bool shouldSkip = true;

        foreach (RangedPoint rp in controller.bossController.enemyManager.rangedPositions.OrderBy(r => Vector3.Distance(controller.transform.position, r.transform.position)))
        {
            if (shouldSkip)
            {
                shouldSkip = false;
                continue;
            }

            if (idx < controller.bossController.boss.rangedPointsToCheck)
            {
                closestPoints[idx] = rp;
                idx++;
            }
            else
                break;
        }

        closestPoints = closestPoints.OrderByDescending(r => Vector3.Distance(player.transform.position, r.transform.position)).ToArray();

        pointToGoTo = closestPoints[0];
        posToRunTo = pointToGoTo.transform.position;
        reachedPos = false;
    }
}
