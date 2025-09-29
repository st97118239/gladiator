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

    [SerializeField] private BoxCollider2D enemyCollider;

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
        if (attackType == AttackType.Sing)
            FindPuddle();
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
}
