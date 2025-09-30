using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public LevelManager levelManager;
    public AbilityManager abilityManager;
    public Player player;
    public List<EnemyController> enemies;
    public BossController boss;
    public List<EnemyController> sirens;
    public List<ProjectileObj> projectiles;
    public List<Root> roots;
    public List<Transform> rangedPositions;
    public Wave currentWave;
    public GameObject emptyEnemyPrefab;
    public GameObject emptyBossPrefab;
    public GameObject emptyProjectilePrefab;
    public GameObject emptyRootPrefab;
    public Transform enemyParent;
    public Transform projectileParent;
    public int enemySpawnIdx;
    public int enemyCount;
    public int maxEnemyCount;
    public int maxProjectileCount;
    public int maxRootCount;

    [SerializeField] private float spawnDelay;

    private int spawnPosIdx;

    private void Awake()
    {
        for (int i = 0; i < levelManager.rangedPointsParent.childCount; i++)
        {
            rangedPositions.Add(levelManager.rangedPointsParent.GetChild(i));
        }
    }

    public void EmptyEnemySpawn()
    {
        GameObject bossObj = Instantiate(emptyBossPrefab, Vector3.zero, Quaternion.identity, enemyParent);
        bossObj.SetActive(false);
        boss = bossObj.GetComponent<BossController>();

        for (int i = 0; i < maxEnemyCount; i++)
        {
            GameObject enemyObj = Instantiate(emptyEnemyPrefab, Vector3.zero, Quaternion.identity, enemyParent);
            enemyObj.SetActive(false);
            enemies.Add(enemyObj.GetComponent<EnemyController>());
        }
    }

    public void EmptyProjectileSpawn()
    {
        for (int i = 0; i < maxProjectileCount; i++)
        {
            GameObject projObj = Instantiate(emptyProjectilePrefab, Vector3.zero, Quaternion.identity, projectileParent);
            projObj.SetActive(false);
            projectiles.Add(projObj.GetComponent<ProjectileObj>());
        }
    }

    public void EmptyRootSpawn()
    {
        for (int i = 0; i < maxRootCount; i++)
        {
            GameObject rootObj = Instantiate(emptyRootPrefab, Vector3.zero, Quaternion.identity, projectileParent);
            rootObj.SetActive(false);
            roots.Add(rootObj.GetComponent<Root>());
        }
    }

    public void SpawnEnemy(Wave givenWave)
    {
        currentWave = givenWave;

        enemyCount = currentWave.enemies.Count;

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        sirens.Clear();

        Debug.Log("Spawning enemies.");

        WaitForSeconds wait = new(spawnDelay);

        enemySpawnIdx = 0;
        int enemyCountToSpawn = enemyCount;

        for (int i = 0; i < enemyCountToSpawn; i++)
        {
            if (i >= maxEnemyCount)
            {
                Debug.LogWarning("Not enough enemy slots available.");
                break;
            }

            EnemyController enemy = enemies[i];
            enemy.Load(currentWave.enemies[i], this);
            if (enemy.enemy.attackType == AttackType.Sing)
            {
                sirens.Add(enemy);
                enemy.transform.position = enemy.enemyStateMachine.puddle.transform.position;
            }
            else
            {
                enemy.transform.position = levelManager.spawnpoints[spawnPosIdx].position;
                enemySpawnIdx++;
            }

            enemy.gameObject.SetActive(true);

            SetSpawnPosIdx();
            yield return wait;
        }

        if (!currentWave.boss) yield break;
        {
            enemyCount++;

            if (enemySpawnIdx >= maxEnemyCount)
            {
                Debug.LogWarning("Not enough enemy slots available.");
                yield break;
            }

            BossController enemy = boss;
            enemy.Load(currentWave.boss, this);
            enemy.transform.position = boss.boss.enemyType == EnemyTypes.Nymph ? Vector3.zero : levelManager.spawnpoints[spawnPosIdx].position;
            enemy.gameObject.SetActive(true);

            enemySpawnIdx++;
            SetSpawnPosIdx();
        }
    }

    private void SetSpawnPosIdx()
    {
        spawnPosIdx++;

        if (spawnPosIdx > levelManager.spawnpoints.Count - 1)
            spawnPosIdx = 0;
    }

    public void CheckIfEnd()
    {
        enemyCount--;

        if (enemyCount != 0) return;
        levelManager.WaveFinish();
    }

    public void SetClosest()
    {
        bool isClosest = true;
        foreach (EnemyController enemy in sirens.OrderBy(e => Vector3.Distance(player.transform.position, e.transform.position)))
        {
            enemy.isClosestSiren = isClosest;
            isClosest = false;
        }
    }

    public void SummonRoot(BossController controller, float angle)
    {
        Root root = roots.FirstOrDefault(r => !r.isOn);
        if (!root) return;
        
        root.transform.position = controller.transform.position;
        root.Load(controller.boss.damage, angle);
    }

    public ProjectileObj GetProjectile()
    {
        return projectiles.FirstOrDefault(proj => !proj.isOn);
    }
}