using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Level level;
    public int currentWave;

    public EnemyManager enemyManger;
    public UIManager uiManager;
    public Player player;

    public List<Transform> spawnpoints;
    public List<Puddle> puddles;
    public List<Platform> platforms;

    public List<Puddle> availablePuddles;

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
        SetPuddles();
        enemyManger.EmptyEnemySpawn();
        enemyManger.EmptyProjectileSpawn();
        enemyManger.EmptyRootSpawn();
        StartCoroutine(LevelCountdown());
    }

    private void SetPuddles()
    {
        availablePuddles.Clear();
        foreach (Puddle puddle in puddles)
        {
            availablePuddles.Add(puddle);
        }
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
        SetPuddles();
        enemyManger.SpawnEnemy(level.waves[currentWave]);
        currentWave++;
    }

    public void WaveFinish()
    {
        Debug.Log("Wave ended.");

        if (level.waves[currentWave - 1].hasAbilityRoll)
        {
            Debug.Log("Showing ability menu");
            uiManager.ShowAbilityMenu();
            return;
        }

        WaveEnd();
    }

    public void WaveEnd()
    {
        if (level.waves.Count > currentWave)
            StartCoroutine(NextWave());
        else
            GameEnd(false);
    }

    private IEnumerator NextWave()
    {
        Debug.Log("Countdown for new wave.");

        yield return waveWait;

        StartWave();
    }

    public void GameEnd(bool died)
    {
        Debug.Log("Game finished.");

        if (!died)
            gameFinishedPanel.SetActive(true);

        player.movementScript.canMove = false;
        player.canAttack = false;
        StartCoroutine(StopGame());
    }

    private IEnumerator StopGame()
    {
        Debug.Log("Exiting game.");

        yield return new WaitForSeconds(gameEndDelay);

        uiManager.Exit();
    }
}
