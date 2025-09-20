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
    [SerializeField] private float gameEndDelay;
    [SerializeField] private GameObject countdownBox;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject gameFinishedPanel;

    private WaitForSeconds waveWait;
    private float countdown;

    private void Start()
    {
        waveWait = new(waveStartDelay);
        int pos = Random.Range(0, spawnpoints.Count);
        player.transform.position = spawnpoints[pos].position;

        gameFinishedPanel.SetActive(false);
        StartLevel();
    }

    private void StartLevel()
    {
        enemyManger.EmptyEnemySpawn();
        enemyManger.EmptyProjectileSpawn();
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

            levelStartDelay--;
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
            GameEnd();
    }

    private IEnumerator NextWave()
    {
        Debug.Log("Countdown for new wave.");

        yield return waveWait;

        StartWave();
    }

    public void GameEnd()
    {
        Debug.Log("Game finished.");
        gameFinishedPanel.SetActive(true);
        player.movementScript.canMove = false;
        player.canAttack = true;
        StartCoroutine(StopGame());
    }

    private IEnumerator StopGame()
    {
        Debug.Log("Exiting game.");

        yield return new WaitForSeconds(gameEndDelay);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
