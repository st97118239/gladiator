using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class JumpWalkState : IEnemyState
{
    private Vector3 posToGoTo;
    private Vector3 otherBridgeToGoTo;
    private Vector3 platformToGoTo;
    private float speed;

    private bool isGoingToBridge;
    private bool isGoingToOtherBridge;
    private bool isGoingToPlatform;
    private bool reachedPos;

    public void UpdateState(EnemyStateMachine controller)
    {
        if (controller.enemyController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        if (controller.enemyController.enemy.attackType != AttackType.Jump) return;

        float posDistance = Vector3.Distance(controller.transform.position, posToGoTo);

        if (posDistance <= controller.enemyController.enemy.attackRadius)
        {
            if (isGoingToBridge)
            {
                reachedPos = true;
                isGoingToBridge = false;
                isGoingToOtherBridge = true;
                isGoingToPlatform = false;
                CalculateNextPath(controller);
                return;
            }

            if (isGoingToOtherBridge)
            {
                if (controller.currentPlatform == controller.platform)
                {
                    reachedPos = true;
                    isGoingToBridge = false;
                    isGoingToOtherBridge = false;
                    isGoingToPlatform = true;
                }
                else
                {
                    reachedPos = true;
                    isGoingToBridge = true;
                    isGoingToOtherBridge = false;
                    isGoingToPlatform = false;
                }

                CalculateNextPath(controller);
                return;
            }

            if (isGoingToPlatform)
            {
                reachedPos = true;
                isGoingToBridge = false;
                isGoingToOtherBridge = false;
                isGoingToPlatform = false;

                if (controller.currentPlatform == controller.platform)
                {
                    controller.reachedPlatform = true;
                    controller.ChangeState(controller.jumpState);
                    return;
                }
            }
        }

        if (!reachedPos)
            controller.transform.position = Vector3.MoveTowards(controller.transform.position, posToGoTo, speed * Time.deltaTime);

        if (posToGoTo.x < controller.transform.position.x)
            controller.enemyController.spriteRenderer.flipX = true;
        else if (posToGoTo.x > controller.transform.position.x) 
            controller.enemyController.spriteRenderer.flipX = false;
    }
    
    public void OnEnter(EnemyStateMachine controller)
    {
        controller.FindPlatform();
        posToGoTo = Vector3.zero;
        otherBridgeToGoTo = Vector3.zero;
        platformToGoTo = Vector3.zero;
        speed = controller.enemyController.enemy.speed;
        isGoingToBridge = true;
        isGoingToOtherBridge = false;
        isGoingToPlatform = false;
        reachedPos = false;
        CalculateNextPath(controller);
    }

    private void CalculateNextPath(EnemyStateMachine controller)
    { 
        if (isGoingToBridge)
        {
            BridgePoint toGoTo = null;

            foreach (BridgePoint bp in controller.currentPlatform.bridges)
            {
                if (bp.otherBridgePoint.isBroken) continue;

                if (bp.otherBridgePoint.platform == controller.platform)
                {
                    toGoTo = bp;
                    break;
                }

                foreach (BridgePoint bp2 in bp.otherBridgePoint.platform.bridges)
                {
                    if (bp2.otherBridgePoint.platform != controller.platform) continue;

                    toGoTo = bp;
                    break;
                }
            }

            if (toGoTo == null)
            {
                Debug.Log("Can't find a bridge to go to. Stopping movement.");
                speed = 0;
                controller.ChangeState(controller.stunnedState);
                return;
            }

            posToGoTo = toGoTo.bridgeEntrance.position;
            otherBridgeToGoTo = toGoTo.otherBridgePoint.bridgeEntrance.position;
            platformToGoTo = toGoTo.otherBridgePoint.platform.middleObj.position;
            reachedPos = false;
        }
        else if (isGoingToOtherBridge)
        {
            posToGoTo = otherBridgeToGoTo;
            reachedPos = false;

        }
        else if (isGoingToPlatform)
        {
            posToGoTo = platformToGoTo;
            reachedPos = false;
        }
    }

    public void OnExit(EnemyStateMachine controller)
    {

    }

    public void OnHurt(EnemyStateMachine controller)
    {

    }
}
