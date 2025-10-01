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
    public BossWalk walkState = new();
    public BossMelee meleeState = new();
    public NymphAttack nymphAttackState = new();
    public BossDash dashState = new();

    public AttackType attackType;
    public BossAbility abilityType;
    public float attackDelay;

    public Puddle puddle;

    public bool isReloading;
    public bool canDash;

    public float dashDelay;
    public float dashCooldown;
    public float dashSpeed;
    public float dashTime;
    public bool isDashing;

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
            dashDelay = dashCooldown / 2;
            dashSpeed = bossController.boss.abilityPower;
            dashTime = bossController.boss.abilityTime;
            Invoke(nameof(ResetDashCooldown), dashCooldown);
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

    public ProjectileObj GetProjectile()
    {
        return bossController.enemyManager.projectiles.FirstOrDefault(proj => !proj.isOn);
    }

    public void StartAttackDelay()
    {
        StartCoroutine(nameof(AttackAnim));
    }

    private IEnumerator AttackAnim()
    {
        isReloading = true;
        bossController.spriteRenderer.color = Color.gray4;

        yield return new WaitForSeconds(bossController.boss.attackSpeed);

        bossController.spriteRenderer.color = Color.white;
        isReloading = false;

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
        spriteRenderer.color = Color.deepSkyBlue;

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
        spriteRenderer.color = Color.white;
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
}
