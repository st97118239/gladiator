using System;
using System.Collections;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public EnemyController enemyController;
    public IEnemyState currentState { get; private set; }

    public IdleState idleState = new();
    public WalkState walkState = new();
    public RangedWalkState rangedWalkState = new();
    public RangedAttackState rangedAttackState = new();
    public AttackState attackState = new();

    public AttackType attackType;
    public float attackDelay;

    private void Start()
    {
        ChangeState(idleState);
        attackType = enemyController.enemy.attackType;
    }

    private void Update()
    {
        currentState?.UpdateState(this);
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
        WaitForSeconds wait = new(enemyController.enemy.attackSpeed);

        yield return wait;

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
                break;
            default:
                Debug.LogWarning("Enemy has no attack type. Please fix!");
                break;
        }
    }
}
