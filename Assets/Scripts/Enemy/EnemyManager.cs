using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public LevelManager levelManager;
    public GameObject playerObj;
    public List<EnemyController> enemies;
    public GameObject emptyEnemyPrefab;
    public Transform enemyParent;
    public int enemySpawnIdx;
    public int enemyCount;
    public int maxEnemyCount;

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
        enemyCount = givenWave.enemies.Count;

        for (int i = 0; i < enemyCount; i++)
        {
            EnemyController enemy = enemies[i];
            enemy.Load(givenWave.enemies[enemySpawnIdx]);
            enemy.gameObject.SetActive(true);

            enemySpawnIdx++;
        }
    }
}