using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyManager enemyManager { get; private set; }
    public EnemyStateMachine enemyStateMachine;
    public Enemy enemy;
    public int health;
    public bool isSummoned;

    public SpriteRenderer spriteRenderer;
    public float hitColorTime;

    public bool isClosestSiren;

    private EnemyHealthBar healthBar;
    private bool hasBeenHit;

    public void Load(Enemy givenEnemy, EnemyManager givenManager, EnemyHealthBar givenHealthBar, bool gotSummoned)
    {
        isSummoned = gotSummoned;
        enemyManager = givenManager;
        enemy = givenEnemy;
        health = enemy.health;
        spriteRenderer.sprite = enemy.sprite;
        enemyStateMachine.isReloading = false;
        healthBar = givenHealthBar;
        hasBeenHit = false;
        enemyStateMachine.Load();
    }

    public void Hit(int damage, bool fromPlayer)
    {
        if (enemyStateMachine.isDashing || enemyStateMachine.isBeingHeld) return;

        enemyStateMachine.currentState.OnHurt(enemyStateMachine);

        if (enemyStateMachine.isBlocking || enemyStateMachine.isDashing) return;
        
        health -= damage;
        enemyManager.levelManager.uiManager.audioManager.PlayEnemyHit();

        if (fromPlayer && enemyManager.abilityManager.hasLifesteal)
            enemyManager.player.Lifesteal(damage);

        if (gameObject.activeSelf)
            StartCoroutine(HitEffect());

        if (!hasBeenHit)
        {
            healthBar.Set(this);
            hasBeenHit = true;
        }

        healthBar.GotHit(health);

        if (health > 0) return;

        healthBar.Stop();
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
        spriteRenderer.color = enemyManager.defaultEnemyColor;
        if (enemy.attackType == AttackType.Sing)
        {
            enemyManager.sirens.Remove(this);
            isClosestSiren = false;
        }

        if (enemyStateMachine.platform)
        {
            if (!enemyStateMachine.reachedPlatform) 
                enemyManager.levelManager.availablePlatforms.Add(enemyStateMachine.platform);
            enemyStateMachine.platform = null;
        }

        enemyStateMachine.currentPlatform = null;

        enemyManager.CheckIfEnd(isSummoned, fromPlayer ? enemy.healthPotionChance : 0);
    }

    private IEnumerator HitEffect()
    {
        spriteRenderer.color = enemyManager.hitEnemyColor;

        yield return new WaitForSeconds(hitColorTime);

        switch (enemyStateMachine.isReloading)
        {
            case true:
                spriteRenderer.color = enemyManager.cooldownEnemyColor;
                break;
            case false:
                spriteRenderer.color = enemyStateMachine.isBlocking ? enemyManager.cooldownEnemyColor : enemyManager.defaultEnemyColor;
                break;
        }
    }

    public void Stun(float duration)
    {
        enemyStateMachine.ChangeState(enemyStateMachine.stunnedState);

        StartCoroutine(StunTime(duration));
    }

    private IEnumerator StunTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        enemyStateMachine.ChangeState(enemyStateMachine.idleState);
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.gameObject.CompareTag("FallTrigger") && enemy.canFall)
            Hit(10000, false);
    }
}
