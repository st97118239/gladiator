using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public LevelManager levelManager;
    public Player player;
    public List<EnemyController> enemies;
    public List<Vector3> rangedPositions;
    public Wave currentWave;
    public GameObject emptyEnemyPrefab;
    public Transform enemyParent;
    public int enemySpawnIdx;
    public int enemyCount;
    public int maxEnemyCount;

    [SerializeField] private List<Vector3> spawnPositions;
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

    public void SpawnEnemy(Wave givenWave)
    {
        currentWave = givenWave;

        enemyCount = currentWave.enemies.Count;

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        WaitForSeconds wait = new(spawnDelay);

        for (int i = 0; i < enemyCount; i++)
        {
            EnemyController enemy = enemies[i];
            enemy.Load(currentWave.enemies[enemySpawnIdx], this);
            enemy.transform.position = spawnPositions[spawnPosIdx];
            enemy.gameObject.SetActive(true);

            enemySpawnIdx++;
            SetSpawnPosIdx();
            yield return wait;
        }
    }

    private void SetSpawnPosIdx()
    {
        spawnPosIdx++;

        if (spawnPosIdx > spawnPositions.Count - 1)
            spawnPosIdx = 0;
    }
}