using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

public class BossStateMachine : MonoBehaviour
{
    public BossController bossController;
    public IBossState currentState { get; private set; }

    public BossIdle idleState = new();
    public NymphIdle nymphIdleState = new();
    public NymphWalk nymphWalkState = new();
    public ZeusWalk zeusWalkState = new();
    public GriffonFly griffonFlyState = new();
    public BossWalk walkState = new();
    public BossMelee meleeState = new();
    public NymphAttack nymphAttackState = new();
    public ZeusThrow zeusAttackState = new();
    public GriffonSwipe griffonSwipeState = new();
    public BossDash dashState = new();
    public ZeusLightning zeusLightningState = new();
    public BossStunned stunnedState = new();
    public BossFreeze bossFreezeState = new();

    public AttackType attackType;
    public BossAbility abilityType;
    public float attackDelay;

    public Puddle puddle;
    public Platform currentPlatform;
    public bool isOnPlatform;

    public bool isReloading;
    public bool canDash;

    public float abilityRadius;
    public float abilityDelay;
    public float abilityCooldown;
    public float abilityPower;
    public float abilityTime;
    public bool isUsingAbility;
    public bool isStunned;
    public bool isFrozen;
    public bool canBeHit;

    public BoxCollider2D bossCollider;
    public Rigidbody2D rb2d;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3 gizmoHitboxScale;

    private void Awake()
    {
        if (Application.isPlaying)
            gizmoHitboxScale = bossCollider.size * transform.localScale;
    }

    private void Update()
    {
        currentState?.UpdateState(this);
    }

    public void Load()
    {
        if (bossController.boss.abilityUseChance > 100) 
            Debug.LogWarning("Boss Ability Use Chance should not be above 100. Please fix!");

        attackType = bossController.boss.attackType;
        abilityType = bossController.boss.abilityType;
        canDash = false;
        isUsingAbility = false;

        if (attackType == AttackType.Sing)
            FindPuddle();

        switch (abilityType)
        {
            case BossAbility.Dash:
                canDash = true;
                abilityCooldown = bossController.boss.abilityCooldown;
                abilityDelay = abilityCooldown;
                abilityPower = bossController.boss.abilityPower;
                abilityTime = bossController.boss.abilityTime;
                abilityRadius = bossController.boss.abilityRadius;
                Invoke(nameof(ResetAbilityCooldown), abilityCooldown / 2);
                break;
            case BossAbility.Summon:
                abilityCooldown = bossController.boss.abilityCooldown;
                abilityDelay = abilityCooldown;
                abilityPower = bossController.boss.abilityPower;
                abilityTime = bossController.boss.abilityTime;
                abilityRadius = bossController.boss.abilityRadius;
                Invoke(nameof(ResetAbilityCooldown), abilityCooldown / 2);
                break;
        }

        canBeHit = true;
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        ChangeState(idleState);
    }

    public void ChangeState(IBossState newState)
    {
        currentState?.OnExit(this);
        currentState = newState;
        currentState.OnEnter(this);
    }

    public void StartAttackDelay(bool isExtraAttack)
    {
        StartCoroutine(AttackAnim(isExtraAttack));
    }

    private IEnumerator AttackAnim(bool isExtraAttack)
    {
        isReloading = true;
        bossController.spriteRenderer.color = bossController.enemyManager.cooldownEnemyColor; // Sprite Color
        StopCoroutine(AbilityCooldown());
        abilityDelay = 1;

        yield return isExtraAttack
            ? new WaitForSeconds(bossController.boss.extraAttackSpeed)
            : new WaitForSeconds(bossController.boss.attackSpeed);

        bossController.spriteRenderer.color = bossController.enemyManager.defaultEnemyColor; // Sprite Color
        isReloading = false;

        if (isStunned || isFrozen) yield break;
        StartCoroutine(AbilityCooldown());
        ChangeState(idleState);
    }

    public void FindPuddle()
    {
        if (bossController.enemyManager.levelManager.availablePuddles.Count == 0)
        {
            Debug.LogError("Not enough puddles. Please fix!");
            bossController.enemyManager.levelManager.uiManager.Quit();
        }

        puddle = bossController.enemyManager.levelManager.availablePuddles[Random.Range(0, bossController.enemyManager.levelManager.availablePuddles.Count)];
        bossController.enemyManager.levelManager.availablePuddles.Remove(puddle);

        if (!puddle)
        {
            Debug.LogError("Not enough puddles. Please fix!");
            bossController.enemyManager.levelManager.uiManager.Quit();
        }

        puddle.occupied = true;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.blueViolet;
        Gizmos.DrawWireSphere(transform.position, bossController.boss.attackRadius);

        Gizmos.color = Color.darkBlue;
        Gizmos.DrawLine(transform.position, bossController.enemyManager.player.transform.position);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, gizmoHitboxScale);
    }
    public void Dash()
    {
        if (!canDash && abilityDelay >= 0) return;

        Vector3 moveAmount = (bossController.enemyManager.player.transform.position - transform.position).normalized;

        if (moveAmount == Vector3.zero) return;

        isUsingAbility = true;
        canBeHit = false;
        spriteRenderer.color = bossController.enemyManager.dashEnemyColor; // Sprite Color

        ChangeState(dashState);
        rb2d.AddForce(moveAmount * abilityPower, ForceMode2D.Force);
        rb2d.linearDamping = 5;

        Invoke(nameof(ResetRigidbody), abilityTime);
        StartCoroutine(AbilityCooldown());
    }

    public bool CanSummon()
    {
        if (abilityDelay > 0) return false;

        int enemiesOff = bossController.enemyManager.summonerEnemies.Count(enemy => !enemy.isActiveAndEnabled);

        return enemiesOff >= bossController.boss.enemyAmtToSpawn;
    }

    public void StartSummonCountdown()
    {
        isReloading = true;
        bossController.spriteRenderer.color = bossController.enemyManager.summonEnemyColor; // Sprite Color
        StopCoroutine(AbilityCooldown());
        abilityDelay = 1;
        ChangeState(bossFreezeState);
        StartCoroutine(bossController.enemyManager.SpawnSummonerEnemies(bossController.boss.enemyAmtToSpawn, bossController.boss.enemyToSummon, this));
    }

    public IEnumerator SummonAnim()
    {
        yield return new WaitForSeconds(bossController.boss.abilityTime);

        bossController.spriteRenderer.color = bossController.enemyManager.defaultEnemyColor; // Sprite Color
        isReloading = false;

        if (isStunned) yield break;
        StartCoroutine(AbilityCooldown());
        ChangeState(idleState);
    }

    private void ResetRigidbody()
    {
        rb2d.linearDamping = 10;
        isUsingAbility = false;
        canBeHit = false;
        spriteRenderer.color = bossController.enemyManager.defaultEnemyColor; // Sprite Color
        ChangeState(idleState);
    }

    public IEnumerator AbilityCooldown()
    {
        abilityDelay = abilityCooldown;

        yield return new WaitForSeconds(abilityDelay);

        abilityDelay = -1;
    }

    private void ResetAbilityCooldown()
    {
        abilityDelay = -1;
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        if (hit.gameObject.CompareTag("Platform"))
        {
            currentPlatform = hit.gameObject.GetComponent<Platform>();
            StartCoroutine(ReachesPlatform(currentPlatform));
        }
    }

    private IEnumerator ReachesPlatform(Platform givenPlatform)
    {
        yield return new WaitForSeconds(0.7f);

        if (currentPlatform != givenPlatform) yield break;

        isOnPlatform = true;
    }

    private void OnTriggerExit2D(Collider2D hit)
    {
        if (hit.gameObject.CompareTag("Platform"))
        {
            currentPlatform = null;
            isOnPlatform = false;
        }
    }

    public void ZeusLightningAbility()
    {
        canBeHit = false;
        StartCoroutine(ZeusStartAbility());
    }

    private IEnumerator ZeusStartAbility()
    {
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            transform.position += Vector3.up * 0.007f;
            transform.localScale += new Vector3(0.002f, 0.002f, 0);
            yield return null;
        }

        // TO-DO: Make these variables a normal variable in this or boss script.
        bossController.enemyManager.lightningStrikes[0].Load(bossController.enemyManager.player, Mathf.RoundToInt(bossController.boss.abilityPower));

        float chargeTime = 3;
        for (float i = 0; i < chargeTime; i += Time.deltaTime)
        {
            if (i > chargeTime) i = chargeTime;

            float fillAmount = i / chargeTime;

            spriteRenderer.color = Color.Lerp(bossController.enemyManager.defaultEnemyColor, bossController.enemyManager.dashEnemyColor, fillAmount);

            yield return null;
        }

        yield return new WaitForSeconds(1);

        bossController.enemyManager.lightningStrikes[0].StopFollowing();
    }
}
