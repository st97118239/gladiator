using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Level level;
    public int currentWave;

    public EnemyManager enemyManger;

    private void Start()
    {
        StartLevel();
    }

    private void StartLevel()
    {
        enemyManger.EmptyEnemySpawn();
        StartWave();
    }

    public void StartWave()
    {
        enemyManger.SpawnEnemy(level.waves[currentWave]);
        currentWave++;
    }
}
