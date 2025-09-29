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
    public AttackState attackState = new();
    public RangedAttackState rangedAttackState = new();
    public BlockState blockState = new();

    public AttackType attackType;
    public float attackDelay;

    public Puddle puddle;

    public bool isReloading;
    public bool isBlocking;
    public bool canBlock;

    public BoxCollider2D enemyCollider;

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
        switch (attackType)
        {
            case AttackType.Sing:
                FindPuddle();
                break;
            case AttackType.MeleeBlock:
                canBlock = true;
                break;
        }
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
        enemyController.spriteRenderer.color = Color.gray4;

        yield return new WaitForSeconds(enemyController.enemy.attackSpeed);

        switch (isBlocking)
        {
            case true:
                enemyController.spriteRenderer.color = Color.gray4;
                break;
            case false:
                enemyController.spriteRenderer.color = Color.white;
                break;
        }
        isReloading = false;

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
        enemyController.spriteRenderer.color = Color.gray4;

        yield return new WaitForSeconds(enemyController.enemy.blockTime);

        switch (isReloading)
        {
            case true:
                enemyController.spriteRenderer.color = Color.gray4;
                break;
            case false:
                enemyController.spriteRenderer.color = Color.white;
                break;
        }

        isBlocking = false;

        ChangeState(idleState);

        yield return new WaitForSeconds(enemyController.enemy.blockCooldown - enemyController.enemy.blockTime);

        canBlock = true;
    }

    public void FindPuddle()
    {
        if (enemyController.enemyManager.levelManager.availablePuddles.Count == 0)
        {
            Debug.LogError("Not enough puddles. Please fix!");
            enemyController.enemyManager.levelManager.uiManager.Quit();
        }

        puddle = enemyController.enemyManager.levelManager.availablePuddles[Random.Range(0, enemyController.enemyManager.levelManager.availablePuddles.Count)];
        enemyController.enemyManager.levelManager.availablePuddles.Remove(puddle);

        if (!puddle)
        {
            Debug.LogError("Not enough puddles. Please fix!");
            enemyController.enemyManager.levelManager.uiManager.Quit();
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
