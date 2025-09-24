using UnityEngine;

public class AttackState : IEnemyState
{
    public void UpdateState(EnemyStateMachine controller)
    {
        
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        Attack(controller.enemyController.enemyManager.player, controller.enemyController, controller);
    }

    private static void Attack(Player player, EnemyController enemyController, EnemyStateMachine controller)
    {
        player.PlayerHit(enemyController.enemy.damage, true);
        controller.StartAttackDelay();
    }

    public void OnExit(EnemyStateMachine controller)
    {
        
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
