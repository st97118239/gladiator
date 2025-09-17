using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Level level;
    public int currentWave;

    public EnemyManager enemyManger;
    public Player player;

    [SerializeField] private List<Transform> spawnpoints;

    [SerializeField] private int levelStartDelay;
    [SerializeField] private int waveStartDelay;
    [SerializeField] private GameObject countdownBox;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject gameFinishedPanel;

    private float countdown;

    private void Start()
    {
        int pos = Random.Range(0, spawnpoints.Count);
        player.transform.position = spawnpoints[pos].position;

        gameFinishedPanel.SetActive(false);
        StartLevel();
    }

    private void StartLevel()
    {
        enemyManger.EmptyEnemySpawn();
        StartCoroutine(LevelCountdown());
    }

    private IEnumerator LevelCountdown()
    {
        WaitForSeconds wait1Second = new(1);

        countdownBox.SetActive(true);

        while (levelStartDelay > 0)
        {
            countdownText.text = levelStartDelay.ToString();

            yield return wait1Second;

            levelStartDelay -= 1;
        }

        countdownBox.SetActive(false);

        StartWave();
    }

    private void StartWave()
    {
        enemyManger.SpawnEnemy(level.waves[currentWave]);
        currentWave++;
    }

    public void WaveEnd()
    {
        Debug.Log("Wave ended.");

        if (level.waves.Count > currentWave)
            StartCoroutine(NextWave());
        else
        {
            Debug.Log("Game finished.");
            gameFinishedPanel.SetActive(true);
        }
    }

    private IEnumerator NextWave()
    {
        Debug.Log("Countdown for new wave.");

        WaitForSeconds wait1Second = new(1);

        countdown = waveStartDelay;

        while (countdown > 0)
        {
            yield return wait1Second;

            countdown -= 1;
        }

        StartWave();
    }
}
