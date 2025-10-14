using UnityEngine;

public class JumpWalkState : IEnemyState
{
    private Platform platform;
    private Vector3 posToGoTo;
    private Vector3 bridgeToGoTo;
    private Vector3 platformToGoTo;
    private bool isGoingToPlatform;
    private float speed;

    public void UpdateState(EnemyStateMachine controller)
    {
        if (controller.enemyController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        if (bridgeToGoTo == Vector3.zero && platformToGoTo == Vector3.zero)
            CalculatePath(controller);

        Vector3 position = controller.transform.position;

        if (Vector3.Distance(position, bridgeToGoTo) <= controller.enemyController.enemy.attackRadius)
        {
            Debug.Log("Reached bridge."); // DEBUGGING
            bridgeToGoTo = Vector3.zero;
            isGoingToPlatform = true;
            posToGoTo = platformToGoTo;
        }
        else if (Vector3.Distance(position, platformToGoTo) <= controller.enemyController.enemy.attackRadius)
        {
            Debug.Log("Reached platform."); // DEBUGGING
            isGoingToPlatform = false;
            CalculatePath(controller);
        }
        
        if (Vector3.Distance(position, platform.middleObj.position) <= controller.enemyController.enemy.attackRadius)
        {
            Debug.Log("Reached destination."); // DEBUGGING
            controller.reachedPlatform = true;
            controller.ChangeState(controller.jumpState);
            return;
        }

        controller.transform.position = Vector3.MoveTowards(controller.transform.position, posToGoTo, speed * Time.deltaTime);

        if (posToGoTo.x < controller.transform.position.x)
            controller.enemyController.spriteRenderer.flipX = true;
        else if (posToGoTo.x > controller.transform.position.x) 
            controller.enemyController.spriteRenderer.flipX = false;

        if (controller.canDash && controller.dashDelay < 0)
            controller.Dash();
    }
    
    public void OnEnter(EnemyStateMachine controller)
    {
        controller.FindPlatform();
        posToGoTo = Vector3.zero;
        bridgeToGoTo = Vector3.zero;
        platformToGoTo = Vector3.zero;
        isGoingToPlatform = false;
        platform = controller.platform;
        speed = controller.enemyController.enemy.speed;
        CalculatePath(controller);
    }

    private void CalculatePath(EnemyStateMachine controller)
    {
        if (isGoingToPlatform || !controller.currentPlatform)
        {
            Debug.Log("Can't move."); // DEBUGGING
            return;
        }

        if (platform.middleObj.position == posToGoTo)
        {
            Debug.Log("Already going to a platform."); // DEBUGGING
            isGoingToPlatform = true;
            return;
        }

        if (controller.currentPlatform == platform)
        {
            if (!controller.currentPlatform.isBroken)
            {
                Debug.Log("This platform"); // DEBUGGING
                isGoingToPlatform = true;
                posToGoTo = platform.middleObj.position;
                return;
            }
            Debug.Log("This platform is unavailable."); // DEBUGGING
        }
        if (controller.currentPlatform.platformUp == platform)
        {
            if (!controller.currentPlatform.platformUp.isBroken)
            {
                Debug.Log("Up."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeUp.transform.position;
                platformToGoTo = controller.currentPlatform.platformUp.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Up is unavailable."); // DEBUGGING
        }
        if (controller.currentPlatform.platformDown == platform)
        {
            if (!controller.currentPlatform.platformDown.isBroken)
            {
                Debug.Log("Down."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeDown.transform.position;
                platformToGoTo = controller.currentPlatform.platformDown.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Down is unavailable."); // DEBUGGING
        }
        if (controller.currentPlatform.platformLeft == platform)
        {
            if (!controller.currentPlatform.platformLeft.isBroken)
            {
                Debug.Log("Left."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeLeft.transform.position;
                platformToGoTo = controller.currentPlatform.platformLeft.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Left is unavailable."); // DEBUGGING
        }
        if (controller.currentPlatform.platformRight == platform)
        {
            if (!controller.currentPlatform.platformRight.isBroken)
            {
                Debug.Log("Right."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeRight.transform.position;
                platformToGoTo = controller.currentPlatform.platformRight.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Right is unavailable."); // DEBUGGING
        }
        if (controller.currentPlatform.platformUp.platformRight == platform)
        {
            if (!controller.currentPlatform.platformUp.platformRight.isBroken)
            {
                Debug.Log("Up and Right."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeUp.transform.position;
                platformToGoTo = controller.currentPlatform.platformUp.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Up and Right unavailable."); // DEBUGGING
        }

        if (controller.currentPlatform.platformUp.platformLeft == platform)
        {
            if (!controller.currentPlatform.platformUp.platformLeft.isBroken)
            {
                Debug.Log("Up and Left."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeUp.transform.position;
                platformToGoTo = controller.currentPlatform.platformUp.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Up and Left unavailable."); // DEBUGGING
        }
        if (controller.currentPlatform.platformDown.platformLeft == platform)
        {
            if (!controller.currentPlatform.platformDown.platformLeft.isBroken)
            {
                Debug.Log("Down and Left."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeDown.transform.position;
                platformToGoTo = controller.currentPlatform.platformDown.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Down and Left unavailable."); // DEBUGGING
        }
        if (controller.currentPlatform.platformDown.platformRight == platform)
        {
            if (!controller.currentPlatform.platformDown.platformRight.isBroken)
            {
                Debug.Log("Down and Right."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeDown.transform.position;
                platformToGoTo = controller.currentPlatform.platformDown.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Down and Right unavailable."); // DEBUGGING

        }
        if (controller.currentPlatform.platformLeft.platformUp == platform)
        {
            if (!controller.currentPlatform.platformLeft.platformUp.isBroken)
            {
                Debug.Log("Left and Up."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeLeft.transform.position;
                platformToGoTo = controller.currentPlatform.platformLeft.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Left and Up unavailable."); // DEBUGGING
        }
        if (controller.currentPlatform.platformLeft.platformDown == platform)
        {
            if (!controller.currentPlatform.platformLeft.platformDown.isBroken)
            {
                Debug.Log("Left and Down."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeLeft.transform.position;
                platformToGoTo = controller.currentPlatform.platformLeft.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Left and Down unavailable."); // DEBUGGING
        }
        if (controller.currentPlatform.platformRight.platformUp == platform)
        {
            if (!controller.currentPlatform.platformRight.platformUp.isBroken)
            {
                Debug.Log("Right and Up."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeRight.transform.position;
                platformToGoTo = controller.currentPlatform.platformRight.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Right and Up unavailable."); // DEBUGGING
        }
        if (controller.currentPlatform.platformRight.platformDown == platform)
        {
            if (!controller.currentPlatform.platformRight.platformDown.isBroken)
            {
                Debug.Log("Right and Down."); // DEBUGGING
                bridgeToGoTo = controller.currentPlatform.bridgeRight.transform.position;
                platformToGoTo = controller.currentPlatform.platformRight.middleObj.position;
                isGoingToPlatform = false;
                posToGoTo = bridgeToGoTo;
                return;
            }
            Debug.Log("Right and Down unavailable."); // DEBUGGING
        }

        Debug.Log("Nothing is available."); // DEBUGGING
    }

    public void OnExit(EnemyStateMachine controller)
    {

    }

    public void OnHurt(EnemyStateMachine controller)
    {
        if (controller.attackType == AttackType.MeleeBlock && controller.canBlock)
        {
            controller.ChangeState(controller.blockState);
        }
    }
}
