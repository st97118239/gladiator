using UnityEngine;

public class RangedWalkState : IEnemyState
{
    private Player player;
    private float speed;
    private int posIdx;
    private Vector3 posToGoTo;
    private bool reachedPos;

    // Sling user has to walk to random ranged position, then if player is near, make them walk to another position where player is *not* near

    public void UpdateState(EnemyStateMachine controller)
    {
        if (controller.enemyController.enemyManager.player.isDead)
        {
            controller.ChangeState(controller.idleState);
            return;
        }

        float playerDistance = Vector3.Distance(controller.gameObject.transform.position, player.transform.position);

        if (playerDistance <= controller.enemyController.enemy.attackRadius && playerDistance > controller.enemyController.enemy.rangedFleeRadius)
        {
            controller.ChangeState(controller.rangedAttackState);
            return;
        }

        switch (reachedPos)
        {
            case false when controller.gameObject.transform.position == posToGoTo:
                reachedPos = true;
                break;
            case true when playerDistance <= controller.enemyController.enemy.rangedFleeRadius:
                OnEnter(controller);
                return;
        }

        controller.gameObject.transform.position = Vector3.MoveTowards(controller.gameObject.transform.position, posToGoTo, speed * Time.deltaTime);
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        reachedPos = false;
        player = controller.enemyController.enemyManager.player;
        Debug.Log("Walk away");
        speed = controller.enemyController.enemy.speed;
        posIdx = Random.Range(0, controller.enemyController.enemyManager.rangedPositions.Count - 1);
        Debug.Log(posIdx);
        posToGoTo = controller.enemyController.enemyManager.rangedPositions[posIdx].position;
    }

    public void OnExit(EnemyStateMachine controller)
    {

    }

    public void OnHurt(EnemyStateMachine controller)
    {

    }
}
