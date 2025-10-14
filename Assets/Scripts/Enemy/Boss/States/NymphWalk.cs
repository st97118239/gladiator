using System.Linq;
using UnityEngine;

public class NymphWalk : IBossState
{
    private Player player;
    private float speed;
    private bool reachedPos;
    private bool goToRangedPoint;

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
            reachedPos = false;
            goToRangedPoint = true;
            CalculateNextPath(controller);

            if (playerDistance <= controller.bossController.boss.attackRadius && !controller.isReloading)
            {
                Vector3 aimDir = (controller.bossController.enemyManager.player.transform.position - controller.transform.position).normalized;
                float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
                angle -= 90;

                controller.bossController.enemyManager.SummonRoot(controller.bossController, angle);
                controller.bossController.enemyManager.levelManager.uiManager.audioManager.PlayEnemyCast();

                controller.StartAttackDelay(false);
            }
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
