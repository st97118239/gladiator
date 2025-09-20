using System;
using System.Collections;
using System.Linq;
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

    private void Update()
    {
        currentState?.UpdateState(this);
    }

    public void Load()
    {
        ChangeState(idleState);
        attackType = enemyController.enemy.attackType;
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
                break;
            default:
                Debug.LogWarning("Enemy has no attack type. Please fix!");
                break;
        }
    }
}
