using System;
using System.Collections;
using System.Linq;
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
    public AttackState attackState = new();
    public RangedAttackState rangedAttackState = new();

    public AttackType attackType;
    public float attackDelay;

    public Puddle puddle;

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
        attackType = enemyController.enemy.attackType;
        if (attackType == AttackType.Sing)
            FindPuddle();
        ChangeState(idleState);
    }

    public void ChangeState(IEnemyState newState)
    {
        currentState?.OnExit(this);
        currentState = newState;
        currentState.OnEnter(this);
    }

    public ProjectileObj GetProjectile()
    {
        return enemyController.enemyManager.projectiles.FirstOrDefault(proj => !proj.isOn);
    }

    public void StartAttackDelay()
    {
        StartCoroutine(nameof(AttackAnim));
    }

    private IEnumerator AttackAnim()
    {
        enemyController.spriteRenderer.color = Color.gray4;

        yield return new WaitForSeconds(enemyController.enemy.attackSpeed);

        enemyController.spriteRenderer.color = Color.white;

        switch (enemyController.enemy.attackType)
        {
            case AttackType.Melee:
                ChangeState(walkState);
                break;
            case AttackType.ProjectileRanged:
                ChangeState(rangedWalkState);
                break;
            case AttackType.None:
            case AttackType.Jump:
            case AttackType.Sing:
                ChangeState(sirenSingState);
                break;
            default:
                Debug.LogWarning("Enemy has no attack type. Please fix!");
                break;
        }
    }

    public void FindPuddle()
    {
        if (enemyController.enemyManager.levelManager.availablePuddles.Count == 0)
        {
            Debug.LogError("Not enough puddles. Please fix!");
            enemyController.enemyManager.levelManager.Quit();
        }

        puddle = enemyController.enemyManager.levelManager.availablePuddles[Random.Range(0, enemyController.enemyManager.levelManager.availablePuddles.Count)];
        enemyController.enemyManager.levelManager.availablePuddles.Remove(puddle);

        if (!puddle)
        {
            Debug.LogError("Not enough puddles. Please fix!");
            enemyController.enemyManager.levelManager.Quit();
        }

        puddle.occupied = true;
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
}
