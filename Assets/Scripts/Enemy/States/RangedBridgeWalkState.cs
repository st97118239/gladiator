using UnityEngine;

public class RangedBridgeWalkState : IEnemyState
{
    private Player player;
    private float speed;
    private bool reachedPos;
    private bool goToBridge;
    private bool goToOtherBridge;
    private bool goToRangedPoint;
    private bool isAttacking;
    private Vector3 otherBridgePos;
    private Platform platformToGoTo;

    private Vector3 posToRunTo;

    public void UpdateState(EnemyStateMachine controller)
    {
        if (controller.enemyController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        if (platformToGoTo.isBroken)
        {
            reachedPos = false;
            goToBridge = true;
            goToOtherBridge = false;
            goToRangedPoint = false;
            CalculateNextPath(controller);
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
                goToBridge = true;
                goToOtherBridge = false;
                goToRangedPoint = false;
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
            if (goToBridge)
            {
                reachedPos = true;
                goToBridge = false;
                goToOtherBridge = true;
                goToRangedPoint = false;
                CalculateNextPath(controller);
                return;
            }

            if (goToOtherBridge)
            {
                reachedPos = true;
                goToBridge = false;
                goToOtherBridge = false;
                goToRangedPoint = true;
                CalculateNextPath(controller);
                return;
            }

            if (goToRangedPoint)
            {
                reachedPos = true;
                goToBridge = false;
                goToOtherBridge = false;
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
        goToBridge = true;
        goToOtherBridge = false;
        goToRangedPoint = false;
        CalculateNextPath(controller);
        controller.enemyController.animator.SetBool("Walk", true);
    }

    public void OnExit(EnemyStateMachine controller)
    {
        controller.enemyController.animator.SetBool("Walk", false);
    }

    public void OnHurt(EnemyStateMachine controller)
    {

    }

    private void CalculateNextPath(EnemyStateMachine controller)
    {
        if (goToBridge)
        {
            BridgePoint furthestBridge = null;
            float distance = -1000;

            foreach (BridgePoint bp in controller.currentPlatform.bridges)
            {
                if (bp.otherBridgePoint.isBroken) continue;

                float bpDistance = Vector3.Distance(player.transform.position, bp.transform.position);

                if (distance != -1000 && !(bpDistance > distance)) continue;

                distance = bpDistance;
                furthestBridge = bp;
            }

            if (!furthestBridge)
            {
                Debug.Log("Couldn't find a bridge, aborting pathfinding.");
                return;
            }

            posToRunTo = furthestBridge.bridgeEntrance.position;
            otherBridgePos = furthestBridge.otherBridgePoint.bridgeEntrance.position;
            platformToGoTo = furthestBridge.otherBridgePoint.platform;
        }
        else if (goToOtherBridge)
        {
            posToRunTo = otherBridgePos;
            reachedPos = false;
        }
        else if (goToRangedPoint)
        {
            posToRunTo = platformToGoTo.rangedPoints[Random.Range(0, controller.currentPlatform.rangedPoints.Length)].transform.position;
            reachedPos = false;
        }
    }
}
