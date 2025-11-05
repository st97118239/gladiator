using UnityEngine;

public class BossMelee : IBossState
{
    private static readonly int AltSlash = Animator.StringToHash("AltSlash");
    private static readonly int MeleeSlash = Animator.StringToHash("MeleeSlash");

    public void UpdateState(BossStateMachine controller)
    {
        
    }

    public void OnEnter(BossStateMachine controller)
    {
        controller.bossController.enemyManager.levelManager.uiManager.audioManager.PlayEnemyAttack();

        Vector3 aimDir = (controller.bossController.enemyManager.player.transform.position - controller.transform.position).normalized;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        angle -= 90;

        controller.aimTransform.eulerAngles = new Vector3(0, 0, angle);
        controller.slashAnimator.SetTrigger(controller.bossController.boss.useAltSlash ? AltSlash : MeleeSlash);

        controller.bossController.enemyManager.player.PlayerHit(controller.bossController.boss.damage, true, false);
        controller.StartAttackDelay(false);
    }

    public void OnExit(BossStateMachine controller)
    {
        
    }

    public void OnHurt(BossStateMachine controller)
    {
        
    }
}
