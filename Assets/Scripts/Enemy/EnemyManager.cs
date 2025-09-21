using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class EnemyManager : MonoBehaviour
{
    public LevelManager levelManager;
    public Player player;
    public List<EnemyController> enemies;
    public List<EnemyController> sirens;
    public List<ProjectileObj> projectiles;
    public List<Transform> rangedPositions;
    public Wave currentWave;
    public GameObject emptyEnemyPrefab;
    public GameObject emptyProjectilePrefab;
    public Transform enemyParent;
    public Transform projectileParent;
    public int enemySpawnIdx;
    public int enemyCount;
    public int maxEnemyCount;
    public int maxProjectileCount;

    [SerializeField] private float spawnDelay;

    private int spawnPosIdx;

    public void EmptyEnemySpawn()
    {
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
            enemy.Load(currentWave.enemies[enemySpawnIdx], this);
            if (enemy.enemy.attackType == AttackType.Sing)
            {
                sirens.Add(enemy);
                enemy.transform.position = enemy.enemyStateMachine.puddle.transform.position;
            }
            else
                enemy.transform.position = levelManager.spawnpoints[spawnPosIdx].position;

            enemy.gameObject.SetActive(true);

            enemySpawnIdx++;
            SetSpawnPosIdx();
            yield return wait;
        }
    }

    private void SetSpawnPosIdx()
    {
        spawnPosIdx++;

        if (spawnPosIdx > levelManager.spawnpoints.Count - 1)
            spawnPosIdx = 0;
    }

    public void CheckIfEnd(EnemyController killedEnemy)
    {
        enemyCount--;
        killedEnemy.gameObject.SetActive(false);
        killedEnemy.transform.position = Vector3.zero;
        killedEnemy.spriteRenderer.color = Color.white;
        if (killedEnemy.enemy.attackType == AttackType.Sing)
        {
            sirens.Remove(killedEnemy);
            killedEnemy.isClosestSiren = false;
        }

        if (enemyCount != 0) return;
        levelManager.WaveEnd();
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
}