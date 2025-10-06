using System.Collections;
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

    public AttackType attackType;
    public BossAbility abilityType;
    public float attackDelay;

    public Puddle puddle;
    public Platform currentPlatform;
    public bool isOnPlatform;

    public bool isReloading;
    public bool canDash;

    public float dashRadius;
    public float dashDelay;
    public float dashCooldown;
    public float dashSpeed;
    public float dashTime;
    public bool isDashing;
    public bool isStunned;

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
        isDashing = false;

        if (attackType == AttackType.Sing)
            FindPuddle();

        if (abilityType == BossAbility.Dash)
        {
            canDash = true;
            dashCooldown = bossController.boss.abilityCooldown;
            dashDelay = dashCooldown;
            dashSpeed = bossController.boss.abilityPower;
            dashTime = bossController.boss.abilityTime;
            dashRadius = bossController.boss.abilityRadius;
            Invoke(nameof(ResetDashCooldown), dashCooldown / 2);
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
        StopCoroutine(DashCooldown());
        dashDelay = 1;

        yield return isExtraAttack
            ? new WaitForSeconds(bossController.boss.extraAttackSpeed)
            : new WaitForSeconds(bossController.boss.attackSpeed);

        bossController.spriteRenderer.color = bossController.enemyManager.defaultEnemyColor; // Sprite Color
        isReloading = false;

        if (isStunned) yield break;
        StartCoroutine(DashCooldown());
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
        if (!canDash && dashDelay >= 0) return;

        Vector3 moveAmount = (bossController.enemyManager.player.transform.position - transform.position).normalized;

        if (moveAmount == Vector3.zero) return;

        isDashing = true;
        spriteRenderer.color = bossController.enemyManager.dashEnemyColor; // Sprite Color

        ChangeState(dashState);
        rb2d.AddForce(moveAmount * dashSpeed, ForceMode2D.Force);
        rb2d.linearDamping = 5;

        Invoke(nameof(ResetRigidbody), dashTime);
        StartCoroutine(DashCooldown());
    }

    private void ResetRigidbody()
    {
        rb2d.linearDamping = 10;
        isDashing = false;
        spriteRenderer.color = bossController.enemyManager.defaultEnemyColor; // Sprite Color
        ChangeState(idleState);
    }

    private IEnumerator DashCooldown()
    {
        dashDelay = dashCooldown;

        yield return new WaitForSeconds(dashDelay);

        dashDelay = -1;
    }

    private void ResetDashCooldown()
    {
        dashDelay = -1;
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
