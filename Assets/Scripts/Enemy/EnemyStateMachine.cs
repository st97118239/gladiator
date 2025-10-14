using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyStateMachine : MonoBehaviour
{
    public EnemyController enemyController;
    public IEnemyState currentState { get; private set; }

    public IdleState idleState = new();
    public SirenSingState sirenSingState = new();
    public WalkState walkState = new();
    public RangedWalkState rangedWalkState = new();
    public RangedBridgeWalkState rangedBridgeWalkState = new();
    public JumpWalkState jumpWalkState = new();
    public AttackState attackState = new();
    public RangedAttackState rangedAttackState = new();
    public JumpState jumpState = new();
    public BlockState blockState = new();
    public DashState dashState = new();
    public PickedUpState pickedUpState = new();
    public StunnedState stunnedState = new();

    public AttackType attackType;
    public EnemyAbility ability;
    public float attackDelay;

    public Puddle puddle;
    public Platform platform;
    public Platform currentPlatform;

    public bool isReloading;
    public bool isBlocking;
    public bool canBlock;
    public bool canDash;
    public bool reachedPlatform;

    public float dashDelay;
    public float dashCooldown;
    public float dashSpeed;
    public float dashTime;
    public bool isDashing;
    public bool isBeingHeld;
    public bool isBeingThrown;
    public bool isStunned;

    public Transform aimTransform;
    public SpriteRenderer slashSprite;
    public Animator slashAnimator;
    public BoxCollider2D enemyCollider;
    public Rigidbody2D rb2d;
    public SpriteRenderer spriteRenderer;

    private Vector3 gizmoHitboxScale;
    private int fallDamage;

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
        attackType = enemyController.enemy.attackType;
        ability = enemyController.enemy.ability;
        canBlock = false;
        canDash = false;
        isDashing = false;
        reachedPlatform = false;
        slashSprite.color = Color.clear;
        switch (attackType)
        {
            case AttackType.Sing:
                FindPuddle();
                break;
            case AttackType.Jump:
                FindPlatform();
                break;
            case AttackType.MeleeBlock:
                canBlock = true;
                break;
        }

        if (ability == EnemyAbility.Dash)
        {
            canDash = true;
            dashCooldown = enemyController.enemy.abilityCooldown;
            dashDelay = dashCooldown;
            dashSpeed = enemyController.enemy.abilityPower;
            dashTime = enemyController.enemy.abilityTime;
            Invoke(nameof(ResetDashCooldown), dashCooldown / 2);
        }

        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        Invoke(nameof(Begin), Time.fixedDeltaTime);
    }

    private void Begin()
    {
        ChangeState(idleState);
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.OnExit(this);
        currentState = newState;
        currentState.OnEnter(this);
    }

    public void StartAttackDelay()
    {
        StartCoroutine(nameof(AttackAnim));
    }

    private IEnumerator AttackAnim()
    {
        isReloading = true;
        enemyController.spriteRenderer.color = enemyController.enemyManager.cooldownEnemyColor;

        yield return new WaitForSeconds(enemyController.enemy.attackSpeed);

        switch (isBlocking)
        {
            case true:
                enemyController.spriteRenderer.color = enemyController.enemyManager.cooldownEnemyColor;
                break;
            case false:
                enemyController.spriteRenderer.color = enemyController.enemyManager.defaultEnemyColor;
                break;
        }
        isReloading = false;

        if (isBeingHeld || isStunned) yield break;
        ChangeState(idleState);
    }

    public void StartBlockDelay()
    {
        StartCoroutine(nameof(BlockAnim));
    }

    private IEnumerator BlockAnim()
    {
        isBlocking = true;
        canBlock = false;
        enemyController.spriteRenderer.color = enemyController.enemyManager.cooldownEnemyColor;

        yield return new WaitForSeconds(enemyController.enemy.blockTime);

        switch (isReloading)
        {
            case true:
                enemyController.spriteRenderer.color = enemyController.enemyManager.cooldownEnemyColor;
                break;
            case false:
                enemyController.spriteRenderer.color = enemyController.enemyManager.defaultEnemyColor;
                break;
        }

        isBlocking = false;

        if (!isBeingHeld)
            ChangeState(idleState);

        yield return new WaitForSeconds(enemyController.enemy.blockCooldown - enemyController.enemy.blockTime);

        canBlock = true;
    }

    public void FindPuddle()
    {
        if (enemyController.enemyManager.levelManager.availablePuddles.Count == 0)
        {
            Debug.LogError("Not enough puddles. Please fix!");
            enemyController.Hit(1000);
        }

        puddle = enemyController.enemyManager.levelManager.availablePuddles[Random.Range(0, enemyController.enemyManager.levelManager.availablePuddles.Count)];
        enemyController.enemyManager.levelManager.availablePuddles.Remove(puddle);

        if (!puddle)
        {
            Debug.LogError("Not enough puddles. Please fix!");
            enemyController.Hit(1000);
        }

        puddle.occupied = true;
    }

    public void FindPlatform()
    {
        if (enemyController.enemyManager.levelManager.availablePlatforms.Count == 0)
        {
            Debug.LogError("Not enough platforms. Please fix!");
            enemyController.Hit(1000);
        }

        Platform pf = enemyController.enemyManager.levelManager.availablePlatforms[Random.Range(0, enemyController.enemyManager.levelManager.availablePlatforms.Count)];
        enemyController.enemyManager.levelManager.availablePlatforms.Remove(pf);

        platform = pf;

        if (pf) return;

        Debug.LogError("Not enough platforms. Please fix!");
        enemyController.Hit(1000);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.blueViolet;
        Gizmos.DrawWireSphere(transform.position, enemyController.enemy.attackRadius);

        Gizmos.color = Color.darkBlue;
        Gizmos.DrawLine(transform.position, enemyController.enemyManager.player.transform.position);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, gizmoHitboxScale);
    }

    public void Dash()
    {
        if (!canDash && dashDelay >= 0) return;

        Vector3 moveAmount = (enemyController.enemyManager.player.transform.position - transform.position).normalized;

        if (moveAmount == Vector3.zero) return;

        isDashing = true;
        spriteRenderer.color = enemyController.enemyManager.dashEnemyColor;

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
        spriteRenderer.color = enemyController.enemyManager.defaultEnemyColor;

        if (isBeingHeld) return;
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
        }
    }

    public void GotPickedUp()
    {
        ChangeState(pickedUpState);
    }

    public void GotThrown(float time, int dmg)
    {
        isBeingThrown = true;
        rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
        fallDamage = dmg;
        Invoke(nameof(LandFromThrow), time);
    }

    private void LandFromThrow()
    {
        isBeingThrown = false;
        rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
        ChangeState(idleState);
        enemyController.Hit(fallDamage);
    }
}
