using UnityEngine;

public class AttackState : IEnemyState
{
    private static readonly int AltSlash = Animator.StringToHash("AltSlash");
    private static readonly int MeleeSlash = Animator.StringToHash("MeleeSlash");

    public void UpdateState(EnemyStateMachine controller)
    {
        
    }

    public void OnEnter(EnemyStateMachine controller)
    {
        controller.enemyController.animator.SetTrigger("Attack");
        Vector3 aimDir = (controller.enemyController.enemyManager.player.transform.position - controller.transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle -= 90;

        controller.aimTransform.eulerAngles = new Vector3(0, 0, angle);
        controller.slashAnimator.SetTrigger(controller.enemyController.enemy.useAltSlash ? AltSlash : MeleeSlash);

        controller.enemyController.enemyManager.player.PlayerHit(controller.enemyController.enemy.damage, true, false);
        controller.enemyController.enemyManager.levelManager.uiManager.audioManager.PlayEnemyAttack();
        controller.StartAttackDelay();
    }

    public void OnExit(EnemyStateMachine controller)
    {
        
    }

    public void OnHurt(EnemyStateMachine controller)
    {
        
    }
}
