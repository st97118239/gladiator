using UnityEngine;

public class PickedUpState : IEnemyState
{
    private Player player;
    private Vector3 lookRight;
    private Vector3 lookLeft;

    public void UpdateState(EnemyStateMachine controller)
    {
        if (controller.isBeingThrown) return;
        controller.gameObject.transform.position = player.isLookingRight
            ? player.transform.position + lookRight
            : player.transform.position + lookLeft;
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        player = controller.enemyController.enemyManager.player;
        controller.isBeingHeld = true;
        lookRight = new Vector3(0.5f, 0.4f, 0);
        lookLeft = new Vector3(-0.5f, 0.4f, 0);
    }

    public void OnExit(EnemyStateMachine controller)
    {
        controller.isBeingHeld = false;
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
