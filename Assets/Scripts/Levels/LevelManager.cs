using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public Level level;
    public int currentWave;

    public EnemyManager enemyManager;
    public UIManager uiManager;
    public Player player;

    public Transform spawnpointsParent;
    public Transform rangedPointsParent;
    public Transform bridePointsParent;
    public Transform swipePointsParent;
    public Transform puddlesParent;
    public Transform platformsParent;

    public List<Transform> spawnpoints;
    public List<Puddle> puddles;
    public List<Platform> platforms;

    public List<Puddle> availablePuddles;
    public List<Platform> availablePlatforms;

    [SerializeField] private int levelStartDelay;
    [SerializeField] private int waveStartDelay;
    [SerializeField] private float gameEndDelay;
    [SerializeField] private GameObject countdownBox;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject gameFinishedPanel;
    [SerializeField] private GameObject gameManagerPrefab;

    private WaitForSeconds waveWait;
    private float countdown;

    private void Awake()
    {
        for (int i = 0; i < spawnpointsParent.childCount; i++)
        {
            spawnpoints.Add(spawnpointsParent.GetChild(i));
        }

        for (int i = 0; i < puddlesParent.childCount; i++)
        {
            puddles.Add(puddlesParent.GetChild(i).GetComponent<Puddle>());
        }

        for (int i = 0; i < platformsParent.childCount; i++)
        {
            Platform platform = platformsParent.GetChild(i).GetComponent<Platform>();

            if (!platform.isEmpty)
                platforms.Add(platform);
        }
    }

    private void Start()
    {
        Debug.LogWarning("Remember to remove the wave skip!"); // TODO

        waveWait = new(waveStartDelay);
        int pos = Random.Range(0, spawnpoints.Count);
        player.transform.position = spawnpoints[pos].position;

        gameFinishedPanel.SetActive(false);
        StartLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            foreach (var enemy in enemyManager.enemies)
            {
                if (!enemy.isActiveAndEnabled) continue;
                enemy.Hit(1000, false);
            }

            if (enemyManager.boss.isActiveAndEnabled)
                enemyManager.boss.Hit(1000, false, false);
        }
    }

    private void StartLevel()
    {
        SetPuddles();
        SetPlatforms();
        enemyManager.SpawnEmpties();
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

    private void SetPlatforms()
    {
        availablePlatforms.Clear();
        foreach (Platform platform in platforms)
        {
            availablePlatforms.Add(platform);
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
        enemyManager.SpawnEnemy(level.waves[currentWave]);
        currentWave++;
    }

    public void WaveFinish()
    {
        if (level.waves[currentWave - 1].hasAbilityRoll)
        {
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
        yield return waveWait;

        StartWave();
    }

    public void GameEnd(bool died)
    {
        if (!died)
            uiManager.ShowWinScreen();
        else
            uiManager.ShowDeathScreen();

        player.movementScript.canMove = false;
        player.canAttack = false;
        player.hasAttackPreview = false;
        player.canHeal = false;
        player.abilityManager.canUseSecondary = false;
        player.abilityManager.canUsePowers = false;
    }

    public void SpawnNewGameManager()
    {
        player.abilityManager.gameManager = Instantiate(gameManagerPrefab).GetComponent<GameManager>();
    }
}
