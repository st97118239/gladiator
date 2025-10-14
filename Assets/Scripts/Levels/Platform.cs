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

    private static readonly int BreakAnim = Animator.StringToHash("Break");

    public void Break()
    {
        isBroken = true;

        if (animator)
            animator.SetTrigger(BreakAnim);
        else
            gameObject.SetActive(false);

        foreach (BridgePoint bridge in bridges)
        {
            bridge.gameObject.SetActive(false);
            bridge.isBroken = true;
        }

        foreach (Transform spawnPoint in spawnpointsToRemove)
        {
            levelManager.spawnpoints.Remove(spawnPoint);
        }
        foreach (RangedPoint rangedPoint in rangedPoints)
        {
            levelManager.enemyManager.rangedPositions.Remove(rangedPoint);
        }

        foreach (EnemyController enemy in levelManager.enemyManager.enemies)
        {
            if (!enemy.isActiveAndEnabled) continue;

            if (enemy.enemyStateMachine.currentPlatform == this)
                enemy.Hit(10000);
        }

        if (levelManager.availablePlatforms.Contains(this))
        {
            levelManager.availablePlatforms.Remove(this);
        }
        brokenCollider.gameObject.SetActive(true);
    }
}
