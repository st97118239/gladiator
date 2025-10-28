using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public bool isBroken;
    public bool isEmpty;
    public Transform middleObj;

    public Platform platformUp;
    public Platform platformDown;
    public Platform platformLeft;
    public Platform platformRight;

    public BridgePoint[] bridges;
    public Transform bridgeUp;
    public Transform bridgeDown;
    public Transform bridgeLeft;
    public Transform bridgeRight;

    public RangedPoint[] rangedPoints;

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform[] spawnpointsToRemove;
    [SerializeField] private BoxCollider2D brokenCollider;
    [SerializeField] private BoxCollider2D bc2d;

    [SerializeField] private float timeUntilBroken;

    private List<EnemyController> enemiesToKill;
    private bool shouldKillPlayer;

    private static readonly int BreakAnim = Animator.StringToHash("Break");

    public void Break()
    {
        isBroken = true;

        brokenCollider.gameObject.SetActive(true);

        if (levelManager.player.currentPlatform == this)
        {
            shouldKillPlayer = true;
            levelManager.player.movementScript.canMove = false;
        }

        animator.SetTrigger(BreakAnim);

        foreach (Transform spawnPoint in spawnpointsToRemove)
        {
            levelManager.spawnpoints.Remove(spawnPoint);
        }
        foreach (RangedPoint rangedPoint in rangedPoints)
        {
            levelManager.enemyManager.rangedPositions.Remove(rangedPoint);
        }

        enemiesToKill = new List<EnemyController>();

        foreach (EnemyController enemy in levelManager.enemyManager.enemies)
        {
            if (!enemy.isActiveAndEnabled) continue;

            if (enemy.enemyStateMachine.currentPlatform != this) continue;

            enemiesToKill.Add(enemy);
            enemy.Stun(timeUntilBroken);
        }

        if (levelManager.availablePlatforms.Contains(this))
        {
            levelManager.availablePlatforms.Remove(this);
        }

        Invoke(nameof(FullyBreak), timeUntilBroken);
    }

    private void FullyBreak()
    {
        foreach (BridgePoint bridge in bridges)
        {
            bridge.gameObject.SetActive(false);
            bridge.isBroken = true;
        }

        foreach (EnemyController enemy in enemiesToKill)
        {
            enemy.Hit(10000, false);
        }

        if (shouldKillPlayer)
            levelManager.player.PlayerHit(1000, false, true);
    }
}
