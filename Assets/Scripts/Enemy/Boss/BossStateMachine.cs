using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossStateMachine : MonoBehaviour
{
    public BossController bossController;
    public IBossState currentState { get; private set; }

    public BossIdle idleState = new();
    public NymphIdle nymphIdleState = new();
    public NymphWalk nymphWalkState = new();
    public GriffonFly griffonFlyState = new();
    public BossWalk walkState = new();
    public BossMelee meleeState = new();
    public NymphAttack nymphAttackState = new();
    public GriffonSwipe griffonSwipeState = new();
    public BossDash dashState = new();
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

    [SerializeField] private BoxCollider2D enemyCollider;
    public Rigidbody2D rb2d;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector3 gizmoHitboxScale;

    private void Awake()
    {
        if (Application.isPlaying)
            gizmoHitboxScale = enemyCollider.size * transform.localScale;
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
}
