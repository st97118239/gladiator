using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public EnemyManager enemyManager { get; private set; }
    public BossStateMachine bossStateMachine;
    public Boss boss;
    public int health;

    public SpriteRenderer spriteRenderer;
    public float hitColorTime;

    public bool isHit;
    public bool isClosestSiren;

    public void Load(Boss givenBoss, EnemyManager givenManager)
    {
        enemyManager = givenManager;
        boss = givenBoss;
        health = boss.health;
        spriteRenderer.sprite = boss.sprite;
        bossStateMachine.Load();
    }

    public void Hit(int damage, bool fromProj, bool fromPlayer)
    {
        if ((!bossStateMachine.canBeHit && !fromProj) || (!bossStateMachine.canBeShot && fromProj)) return;

        bossStateMachine.currentState.OnHurt(bossStateMachine);

        if ((!bossStateMachine.canBeHit && !fromProj) || (!bossStateMachine.canBeShot && fromProj)) return;

        isHit = true;
        health -= damage;

        if (health < 0) health = 0;

        enemyManager.bossBarSlider.value = health;
        enemyManager.levelManager.uiManager.audioManager.PlayEnemyHit();

        if (enemyManager.abilityManager.hasLifesteal)
            enemyManager.player.Lifesteal(damage);

        StartCoroutine(HitEffect());

        if (health > 0) return;

        enemyManager.bossBarSlider.gameObject.SetActive(false);
        enemyManager.bossBarText.gameObject.SetActive(false);
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
        spriteRenderer.color = enemyManager.defaultEnemyColor;

        enemyManager.CheckIfEnd(true, false, boss.healthPotionChance);
    }

    private IEnumerator HitEffect()
    {
        spriteRenderer.color = enemyManager.hitEnemyColor;

        yield return new WaitForSeconds(hitColorTime);

        isHit = false;

        if (bossStateMachine.isUsingAbility && boss.abilityType == BossAbility.Thunder) 
            spriteRenderer.color = enemyManager.chargeEnemyColor;
        else
        {
            switch (bossStateMachine.isReloading)
            {
                case true:
                    if (bossStateMachine.isUsingAbility)
                    {
                        switch (boss.abilityType)
                        {
                            case BossAbility.Dash:
                                spriteRenderer.color = enemyManager.dashEnemyColor;
                                break;
                            case BossAbility.Summon:
                                spriteRenderer.color = enemyManager.summonEnemyColor;
                                break;
                        }
                    }
                    else
                        spriteRenderer.color = enemyManager.cooldownEnemyColor;

                    break;
                case false:
                    spriteRenderer.color = enemyManager.defaultEnemyColor;
                    break;
            }
        }
    }

    public void Stun(float duration)
    {
        bossStateMachine.ChangeState(bossStateMachine.stunnedState);

        StartCoroutine(StunTime(duration));
    }

    private IEnumerator StunTime(float duration)
    {
        yield return new WaitForSeconds(duration);

        bossStateMachine.ChangeState(bossStateMachine.idleState);
    }

    public void StartSummonCooldown()
    {
        StartCoroutine(bossStateMachine.AbilityCooldown());
    }
}
