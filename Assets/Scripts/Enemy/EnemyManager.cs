using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public LevelManager levelManager;
    public AbilityManager abilityManager;
    public Slider bossBarSlider;
    public TMP_Text bossBarText;
    public Player player;
    public List<EnemyController> enemies;
    public List<EnemyController> summonerEnemies;
    public BossController boss;
    public List<EnemyController> sirens;
    public List<ProjectileObj> projectiles;
    public List<Net> nets;
    public List<Root> roots;
    public Lightning lightningStrike;
    public List<RangedPoint> rangedPositions;
    public List<Transform> swipePositions;
    public List<BridgePoint> bridgePoints;
    public Wave currentWave;
    public GameObject emptyEnemyPrefab;
    public GameObject emptyBossPrefab;
    public GameObject emptyProjectilePrefab;
    public GameObject emptyNetPrefab;
    public GameObject emptyRootPrefab;
    public GameObject emptyLightningStrikePrefab;
    public Transform enemyParent;
    public Transform projectileParent;
    public int enemySpawnIdx;
    public int enemyCount;
    public int maxEnemyCount;
    public int maxSummonerEnemyCount;
    public int maxProjectileCount;
    public int maxNetCount;
    public int maxRootCount;

    public Color defaultEnemyColor;
    public Color cooldownEnemyColor;
    public Color hitEnemyColor;
    public Color dashEnemyColor;
    public Color summonEnemyColor;
    public Color chargeEnemyColor;

    [SerializeField] private float spawnDelay;
    [SerializeField] private int healthPotionDropChance;

    private int spawnPosIdx;

    private void Awake()
    {
        for (int i = 0; i < levelManager.rangedPointsParent.childCount; i++)
        {
            rangedPositions.Add(levelManager.rangedPointsParent.GetChild(i).GetComponent<RangedPoint>());
        }

        for (int i = 0; i < levelManager.bridePointsParent.childCount; i++)
        {
            bridgePoints.Add(levelManager.bridePointsParent.GetChild(i).GetComponent<BridgePoint>());
        }

        for (int i = 0; i < levelManager.swipePointsParent.childCount; i++)
        {
            swipePositions.Add(levelManager.swipePointsParent.GetChild(i));
        }
    }

    public void SpawnEmpties()
    {
        for (int i = 0; i < maxEnemyCount; i++)
        {
            GameObject enemyObj = Instantiate(emptyEnemyPrefab, Vector3.zero, Quaternion.identity, enemyParent);
            enemyObj.SetActive(false);
            enemies.Add(enemyObj.GetComponent<EnemyController>());
        }

        for (int i = 0; i < maxSummonerEnemyCount; i++)
        {
            GameObject enemyObj = Instantiate(emptyEnemyPrefab, Vector3.zero, Quaternion.identity, enemyParent);
            enemyObj.SetActive(false);
            summonerEnemies.Add(enemyObj.GetComponent<EnemyController>());
        }

        GameObject bossObj = Instantiate(emptyBossPrefab, Vector3.zero, Quaternion.identity, enemyParent);
        bossObj.SetActive(false);
        boss = bossObj.GetComponent<BossController>();

        for (int i = 0; i < maxProjectileCount; i++)
        {
            GameObject projObj = Instantiate(emptyProjectilePrefab, Vector3.zero, Quaternion.identity, projectileParent);
            projObj.SetActive(false);
            projectiles.Add(projObj.GetComponent<ProjectileObj>());
        }

        for (int i = 0; i < maxNetCount; i++)
        {
            GameObject netObj = Instantiate(emptyNetPrefab, Vector3.zero, Quaternion.identity, projectileParent);
            netObj.SetActive(false);
            nets.Add(netObj.GetComponent<Net>());
        }

        for (int i = 0; i < maxRootCount; i++)
        {
            GameObject rootObj = Instantiate(emptyRootPrefab, Vector3.zero, Quaternion.identity, projectileParent);
            rootObj.SetActive(false);
            roots.Add(rootObj.GetComponent<Root>());
        }

        GameObject strikeObj = Instantiate(emptyLightningStrikePrefab, Vector3.zero, Quaternion.identity, projectileParent);
        strikeObj.SetActive(false);
        lightningStrike = strikeObj.GetComponent<Lightning>();
    }

    public void SpawnEnemy(Wave givenWave)
    {
        currentWave = givenWave;

        enemyCount = currentWave.enemies.Count;
        spawnPosIdx = Random.Range(0, levelManager.spawnpoints.Count);

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        sirens.Clear();

        WaitForSeconds wait = new(spawnDelay);

        enemySpawnIdx = 0;
        int enemyCountToSpawn = enemyCount;

        for (int i = 0; i < enemyCountToSpawn; i++)
        {
            if (i >= maxEnemyCount)
            {
                Debug.LogError("Not enough enemy slots available.");
                break;
            }

            if (!currentWave.enemies[i])
            {
                Debug.LogError("Missing enemy. Skipping it instead. Please fix!!");
                continue;
            }

            EnemyController enemy = enemies[i];
            enemy.Load(currentWave.enemies[i], this, false);
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
            bossBarSlider.maxValue = enemy.health;
            bossBarSlider.value = enemy.health;
            bossBarText.text = enemy.boss.name;
            enemy.gameObject.SetActive(true);
            bossBarSlider.gameObject.SetActive(true);
            bossBarText.gameObject.SetActive(true);

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

    public IEnumerator SpawnSummonerEnemies(int amtToSpawn, Enemy enemyToSpawn, BossStateMachine summoner)
    {
        Debug.Log("Spawning summoner enemies.");

        WaitForSeconds wait = new(spawnDelay);

        yield return wait;

        for (int i = 0; i < amtToSpawn; i++)
        {
            if (i >= maxSummonerEnemyCount)
            {
                Debug.LogError("Not enough enemy slots available.");
                break;
            }

            if (!enemyToSpawn)
            {
                Debug.LogError("Missing enemy. Please fix!!");
                break;
            }

            EnemyController enemy = summonerEnemies[i];
            enemy.Load(enemyToSpawn, this, true);
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

            enemyCount++;
            enemy.gameObject.SetActive(true);

            SetSpawnPosIdx();
            yield return wait;
        }

        StartCoroutine(summoner.SummonAnim());
    }

    public void CheckIfEnd(bool isBoss, bool isSummoned)
    {
        enemyCount--;

        if (enemyCount == 0)
            levelManager.WaveFinish();

        if (isBoss)
            player.GetHealthPotion();
        else
        {
            float dropChance = Random.Range(0, 101);

            if (dropChance <= healthPotionDropChance)
                player.GetHealthPotion();
        }

        if (isSummoned && boss.isActiveAndEnabled) 
            boss.StartSummonCooldown();
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

    public Vector3 GetPlatformSwipePos(Vector3 pos)
    {
        Vector3 posToGive = Vector3.zero;
        foreach (Transform swipePos in swipePositions.OrderBy(t => Vector3.Distance(pos, t.position)))
        {
            posToGive = swipePos.position;
            break;
        }

        return posToGive;
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

    public Net GetNet()
    {
        return nets.FirstOrDefault(net => !net.isOn);
    }
}