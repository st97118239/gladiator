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

    public Transform bridgeUp;
    public Transform bridgeDown;
    public Transform bridgeLeft;
    public Transform bridgeRight;

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject[] bridgesToDestroy;
    [SerializeField] private Transform[] spawnpointsToRemove;
    [SerializeField] private Transform[] rangedPointsToRemove;
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

        foreach (GameObject bridge in bridgesToDestroy)
        {
            bridge.SetActive(false);
        }

        foreach (Transform spawnPoint in spawnpointsToRemove)
        {
            levelManager.spawnpoints.Remove(spawnPoint);
        }
        foreach (Transform rangedPoint in rangedPointsToRemove)
        {
            levelManager.enemyManager.rangedPositions.Remove(rangedPoint);
        }

        if (levelManager.availablePlatforms.Contains(this))
        {
            levelManager.availablePlatforms.Remove(this);
        }
        brokenCollider.gameObject.SetActive(true);
    }
}
