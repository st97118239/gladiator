using UnityEngine;

public class RangedAttackState : IEnemyState
{
    // Should grab first unused projectile in storage that moves towards player, when the player is hit it'll deal damage

    public void UpdateState(EnemyStateMachine controller)
    {
        
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        Attack(controller.enemyController.enemyManager.player, controller.enemyController, controller);
    }

    private static void Attack(Player player, EnemyController enemyController, EnemyStateMachine controller)
    {
        Debug.Log("Shoot");
        player.PlayerHit(enemyController.enemy.damage);
        controller.StartAttackDelay();
    }

    public void OnExit(EnemyStateMachine controller)
    {
        
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
