using System.Linq;
using UnityEngine;

public class RangedWalkState : IEnemyState
{
    private Player player;
    private float speed;
    private bool reachedPos;
    private bool goToRangedPoint;
    private bool isAttacking;

    private RangedPoint pointToGoTo;
    private Vector3 posToRunTo;

    public void UpdateState(EnemyStateMachine controller)
    {
        if (controller.enemyController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        float playerDistance = Vector3.Distance(controller.transform.position, player.transform.position);
        float posDistance = Vector3.Distance(controller.transform.position, posToRunTo);

        if (reachedPos)
        {
            if (playerDistance <= controller.enemyController.enemy.rangedFleeRadius)
            {
                reachedPos = false;
                isAttacking = false;
                goToRangedPoint = true;
                CalculateNextPath(controller);
                return;
            }

            if (playerDistance <= controller.enemyController.enemy.attackRadius && !controller.isReloading)
            {
                isAttacking = true;
                controller.ChangeState(controller.rangedAttackState);
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
            controller.enemyController.spriteRenderer.flipX = true;
        else if (player.transform.position.x > controller.transform.position.x)
            controller.enemyController.spriteRenderer.flipX = false;
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        if (isAttacking)
        {
            float playerDistance = Vector3.Distance(controller.transform.position, player.transform.position);
            if (playerDistance <= controller.enemyController.enemy.attackRadius && playerDistance > controller.enemyController.enemy.rangedFleeRadius)
            {
                controller.ChangeState(controller.rangedAttackState);
                isAttacking = true;
                return;
            }
        }

        isAttacking = false;
        reachedPos = false;
        player = controller.enemyController.enemyManager.player;
        speed = controller.enemyController.enemy.speed;
        goToRangedPoint = true;
        CalculateNextPath(controller);
    }

    public void OnExit(EnemyStateMachine controller)
    {

    }

    public void OnHurt(EnemyStateMachine controller)
    {

    }

    private void CalculateNextPath(EnemyStateMachine controller)
    {
        if (!goToRangedPoint) return;

        RangedPoint[] closestPoints = new RangedPoint[controller.enemyController.enemy.rangedPointsToCheck];
        int idx = 0;
        bool shouldSkip = true;

        foreach (RangedPoint rp in controller.enemyController.enemyManager.rangedPositions.OrderBy(r => Vector3.Distance(controller.transform.position, r.transform.position)))
        {
            if (shouldSkip)
            {
                shouldSkip = false;
                continue;
            }

            if (idx < controller.enemyController.enemy.rangedPointsToCheck)
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
