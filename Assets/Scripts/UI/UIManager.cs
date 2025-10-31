using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public Player player;
    public AbilityManager abilityManager;
    public GameManager gameManager;
    public AudioManager audioManager;
    public EventSystem eventSystem;
    public LevelManager levelManager;
    public EnemyManager enemyManager;
    public Image healthPotionImage;
    public TMP_Text healthPotionText;
    public Color healthPotionsUnavailable;
    public int mainMenuScene;
    public int thisScene;
    public int nextScene;
    public Image fadePanel;
    public TutorialPanel tutorialPanel;
    public TMP_Text waveCounterText;
    public Canvas abilityCanvas;
    public CanvasGroup abilityCanvasGroup;
    public GameObject abilityMenuSelectedObj;
    public Canvas pauseCanvas;
    public GameObject pauseMenuSelectedObj;
    public Canvas settingsCanvas;
    public GameObject settingsMenuSelectedObj;
    public GameObject settingsMenuBackSelectedObj;
    public TMP_Text settingsWindowTypeText;
    public Slider settingsSFXVolumeSlider;
    public Slider settingsMusicVolumeSlider;
    public Canvas deathCanvas;
    public CanvasGroup deathCanvasGroup;
    public GameObject deathSelectedObj;
    public Canvas winCanvas;
    public CanvasGroup winCanvasGroup;
    public GameObject winSelectedObj;
    public GameObject mainMenuConfirmPanel;
    public GameObject mainMenuConfirmSelectedObj;
    public GameObject mainMenuConfirmBackSelectedObj;
    public Canvas levelChangeCanvas;
    public CanvasGroup levelChangeCanvasGroup;
    public LevelChangePlayer levelChangePlayer;
    public RectTransform levelChangePlayerRTransform;
    public float levelChangePlayerSpeed;
    public int levelChangeCurrentLvl;
    public Vector2 levelChangePlayerDefaultPos;
    public Canvas victoryCanvas;
    public CanvasGroup victoryCanvasGroup;
    public GameObject victorySelectedObj;
    public Canvas mainMenuCanvas;
    public GameObject mainMenuSelectedObj;
    public Canvas quitConfirmCanvas;
    public GameObject quitMenuSelectedObj;
    public GameObject quitMenuBackSelectedObj;

    public bool isMainGame;
    public bool canPause;
    public bool isInMenu;
    public bool isInSettings;

    public bool cursorVisibility;
    public bool isOnKeyboard;
    public GameObject lastSelectedObj;

    public AbilitySlot[] abilitySlots;
    [SerializeField] private AbilityCard[] abilityCards;
    [SerializeField] private Button confirmButton;

    [SerializeField] private float fadePanelTime;

    [SerializeField] private GameObject[] mainMenuBackgrounds;

    private bool isPaused;
    private bool hasChosenAbility;

    private Coroutine fadeCoroutine;

    private Vector2 mousePos;

    private static readonly int Level1 = Animator.StringToHash("Level");

    private void Awake()
    {
        isOnKeyboard = true;
        gameManager = FindFirstObjectByType<GameManager>();

        if (!gameManager)
            BorderTypeSetting(false);

        if (PlayerPrefs.GetInt("hasPlayed") == 0)
        {
            PlayerPrefs.SetFloat("sfxVolume", 1);
            PlayerPrefs.SetFloat("musicVolume", 1);
            PlayerPrefs.SetInt("hasPlayed", 1);
            Debug.Log("Set audio settings to default.");
        }
        SetVolumeSettings();

        if (levelChangeCurrentLvl == 1 && gameManager)
            gameManager.Reset();

        if (isMainGame) return;

        int amount = PlayerPrefs.GetInt("HighestLevelReached");

        if (amount == 0) amount++;

        List<GameObject> backgrounds = new();

        foreach (GameObject bg in mainMenuBackgrounds)
        {
            if (amount <= 0) break;

            backgrounds.Add(bg);
            amount--;
        }

        int roll = Random.Range(0, backgrounds.Count);

        backgrounds[roll].SetActive(true);
    }

    private void Start()
    {
        if (gameManager)
            gameManager.StartLevel(levelChangeCurrentLvl);
        fadePanel.color = Color.gray2;
        Time.timeScale = 1;
        fadeCoroutine = StartCoroutine(LoadFade(true, -1, false));

        if (!isMainGame)
        {
            Cursor.lockState = CursorLockMode.None;
            lastSelectedObj = mainMenuSelectedObj;
            if (!isOnKeyboard)
                eventSystem.SetSelectedGameObject(mainMenuSelectedObj);
            isInMenu = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
        }

        cursorVisibility = true;
        if (isOnKeyboard)
            Cursor.visible = cursorVisibility;
    }

    private void Update()
    {
        Vector2 newMousePos = Input.mousePosition;

        if (!isOnKeyboard && mousePos != newMousePos) 
            OnKeyboardUsed();

        mousePos = newMousePos;

        if (!Input.GetKeyDown(KeyCode.Equals)) return;
        ResetSave();
    }

    public void ShowAbilityMenu()
    {
        canPause = false;
        player.canAttack = false;
        player.hasAttackPreview = false;
        player.movementScript.canMove = false;
        player.canHeal = false;
        player.abilityManager.canUseSecondary = false;
        player.abilityManager.canUsePowers = false;

        hasChosenAbility = false;

        List<Ability> possibleAbilities = new();

        switch (levelManager.enemyManager.currentWave.abilitySortToRoll)
        {
            default:
            case AbilitySort.None:
                Debug.LogWarning("No abilities to use.");
                break;
            case AbilitySort.Secondary:
                possibleAbilities.AddRange(abilityManager.secondaries);
                break;
            case AbilitySort.Power:
                possibleAbilities.AddRange(abilityManager.powers);
                break;
            case AbilitySort.Passive:
                possibleAbilities.AddRange(abilityManager.passives);
                break;
            case AbilitySort.Random:
                possibleAbilities.AddRange(abilityManager.secondaries);
                possibleAbilities.AddRange(abilityManager.powers);
                possibleAbilities.AddRange(abilityManager.passives);
                break;
        }


        foreach (Ability ability in possibleAbilities.ToList())
        {
            if (abilityManager.abilities.Contains(ability))
                possibleAbilities.Remove(ability);
            else if (abilityManager.secondarySlot >= 0)
            {
                if (abilityManager.secondaries.Contains(ability))
                    possibleAbilities.Remove(ability);
            }
            else if (abilityManager.powersUnlocked >= abilityManager.maxPowers)
            {
                if (abilityManager.powers.Contains(ability))
                    possibleAbilities.Remove(ability);
            }
            else if (abilityManager.passivesUnlocked >= abilityManager.passives.Length)
            {
                if (abilityManager.passives.Contains(ability))
                    possibleAbilities.Remove(ability);
            }
        }

        int idx;

        switch (possibleAbilities.Count)
        {
            case 0:
                if (!player.hasAttackCooldown)
                    player.canAttack = true;
                player.hasAttackPreview = true;
                player.movementScript.canMove = true;
                player.canHeal = true;
                player.abilityManager.canUseSecondary = true;
                player.abilityManager.canUsePowers = true;
                levelManager.WaveEnd();
                Debug.Log("Can't choose any abilities from this pool.");
                return;
            case 1:
                foreach (AbilityCard card in abilityCards)
                {
                    card.gameObject.SetActive(false);
                }

                abilityMenuSelectedObj.SetActive(true);

                AbilityCard defaultCard = abilityMenuSelectedObj.GetComponent<AbilityCard>();

                idx = Random.Range(0, possibleAbilities.Count);

                defaultCard.Load(possibleAbilities[idx]);
                possibleAbilities.RemoveAt(idx);
                defaultCard.gameObject.SetActive(true);
                break;
            case >= 2:
                foreach (AbilityCard card in abilityCards)
                {
                    if (possibleAbilities.Count == 0)
                    {
                        card.gameObject.SetActive(false);
                        break;
                    }

                    idx = Random.Range(0, possibleAbilities.Count);

                    card.Load(possibleAbilities[idx]);
                    possibleAbilities.RemoveAt(idx);
                    card.gameObject.SetActive(true);
                }

                break;
        }

        abilityCanvas.gameObject.SetActive(true);
        StartCoroutine(AbilityMenuAnim());
    }

    private IEnumerator AbilityMenuAnim()
    {
        abilityCanvasGroup.alpha = 1;
        abilityCanvas.gameObject.SetActive(true);
        lastSelectedObj = abilityMenuSelectedObj;
        if (!isOnKeyboard)
            eventSystem.SetSelectedGameObject(lastSelectedObj);
        abilityMenuSelectedObj.GetComponent<UIButton>().OnSelect(null);
        Cursor.lockState = CursorLockMode.None;

        yield return null;

        for (float i = 0; i <= fadePanelTime + Time.unscaledDeltaTime; i += Time.unscaledDeltaTime)
        {
            if (i > fadePanelTime) i = fadePanelTime;

            float fillAmount = i / fadePanelTime;

            abilityCanvasGroup.alpha = fillAmount;

            yield return null;
        }

        abilityCanvasGroup.alpha = 1;
    }

    public void DifferentAbilitySelected()
    {
        foreach (AbilityCard card in abilityCards)
        {
            card.Deselect();
        }

        hasChosenAbility = true;
        confirmButton.interactable = true;
    }

    public void ConfirmAbility()
    {
        if (!hasChosenAbility) return;

        abilityCanvas.gameObject.SetActive(false);
        lastSelectedObj = null;
        eventSystem.SetSelectedGameObject(null);
        Cursor.lockState = CursorLockMode.Confined;

        foreach (AbilityCard card in abilityCards)
        {
            if (card.isSelected)
                abilityManager.NewPower(card.ability, false);

            card.Reset();
        }

        if (!player.hasAttackCooldown)
            player.canAttack = true;
        player.hasAttackPreview = true;
        player.movementScript.canMove = true;
        player.canHeal = true;
        player.abilityManager.canUseSecondary = true;
        player.abilityManager.canUsePowers = true;
        canPause = true;
    }

    public void NewAbility(Ability givenAbility, int idx, bool endWave)
    {
        abilitySlots[idx].Activate(givenAbility);

        if (endWave)
            levelManager.WaveEnd();
    }

    public void ShowTutorial(TutorialInfo givenTutorial)
    {
        if (PlayerPrefs.GetInt("Tutorial" + givenTutorial.id) != 1)
            tutorialPanel.NewInfo(givenTutorial);
    }

    public void PauseMenu()
    {
        if (!canPause || isInSettings) return;

        if (fadePanel.IsActive())
        {
            StopCoroutine(fadeCoroutine);
            fadePanel.gameObject.SetActive(false);
        }

        isPaused = !isPaused;

        pauseCanvas.gameObject.SetActive(isPaused);
        if (!player.hasAttackCooldown && !isPaused)
            player.canAttack = true;
        else
            player.canAttack = false;
        player.movementScript.canMove = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        lastSelectedObj = isPaused ? pauseMenuSelectedObj : null;
        if (!isOnKeyboard)
            eventSystem.SetSelectedGameObject(lastSelectedObj);
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Confined;
    }

    public void Restart()
    {
        fadeCoroutine = StartCoroutine(LoadFade(false, thisScene, false));
    }

    public void Replay()
    {
        if (gameManager) 
            gameManager.Reset();

        fadeCoroutine = StartCoroutine(LoadFade(false, 1, false));
    }

    private IEnumerator LoadFade(bool shouldReverse, int sceneToLoad, bool shouldQuit)
    {
        cursorVisibility = false;
        if (isOnKeyboard)
            Cursor.visible = cursorVisibility;
        Cursor.lockState = CursorLockMode.Confined;
        canPause = false;
        fadePanel.gameObject.SetActive(true);

        yield return null;

        Color startColor = shouldReverse ? Color.gray2 : Color.clear;
        Color endColor = shouldReverse ? Color.clear : Color.gray2;

        for (float i = 0; i <= fadePanelTime + Time.unscaledDeltaTime; i += Time.unscaledDeltaTime)
        {
            if (i > fadePanelTime) i = fadePanelTime;

            float fillAmount = i / fadePanelTime;

            fadePanel.color = Color.Lerp(startColor, endColor, fillAmount);

            yield return null;
        }

        fadePanel.color = endColor;

        if (shouldReverse)
        {
            fadePanel.gameObject.SetActive(false);
            canPause = true;
            
            switch (levelChangeCurrentLvl)
            {
                case 0:
                    audioManager.PlayBGMMainMenu();
                    break;
                case 1:
                    audioManager.PlayBGMColosseum();
                    break;
                case 2:
                    audioManager.PlayBGMForest();
                    break;
                case 3:
                    audioManager.PlayBGMPlatform();
                    break;
                case 4:
                    audioManager.PlayBGMOlympus();
                    break;
            }
        }

        if (shouldQuit)
            Quit();
        else if (sceneToLoad != -1)
        {
            yield return new WaitForSecondsRealtime(1f);
            LoadScene(sceneToLoad);
        }
    }

    private static void LoadScene(int sceneToLoad)
    {
        SceneManager.LoadSceneAsync(sceneToLoad);
    }

    public void LoadSceneFade(int sceneToLoad)
    {
        fadeCoroutine = StartCoroutine(LoadFade(false, sceneToLoad, false));
    }

    public void MainMenu()
    {
        fadeCoroutine = StartCoroutine(LoadFade(false, mainMenuScene, false));
    }

    public void ShowDeathScreen()
    {
        Time.timeScale = 0;
        StartCoroutine(DeathScreenAnim());
    }

    public void ShowWinScreen(bool isLastLevel)
    {
        if (isLastLevel)
            Time.timeScale = 0;
        StartCoroutine(WinScreenAnim());
    }

    private IEnumerator DeathScreenAnim()
    {
        canPause = false;
        deathCanvasGroup.alpha = 1;
        deathCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        lastSelectedObj = deathSelectedObj;
        if (!isOnKeyboard)
            eventSystem.SetSelectedGameObject(lastSelectedObj);

        yield return null;

        for (float i = 0; i <= fadePanelTime + Time.unscaledDeltaTime; i += Time.unscaledDeltaTime)
        {
            if (i > fadePanelTime) i = fadePanelTime;

            float fillAmount = i / fadePanelTime;

            deathCanvasGroup.alpha = fillAmount;

            yield return null;
        }

        deathCanvasGroup.alpha = 1;
    }

    private IEnumerator WinScreenAnim()
    {
        for (float i = 0; i < 0.3; i += Time.deltaTime)
        {
            yield return null;
        }

        player.canAttack = false;
        canPause = false;
        winCanvasGroup.alpha = 1;
        winCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        lastSelectedObj = winSelectedObj;
        if (!isOnKeyboard)
            eventSystem.SetSelectedGameObject(lastSelectedObj);

        yield return null;

        for (float i = 0; i <= fadePanelTime + Time.unscaledDeltaTime; i += Time.unscaledDeltaTime)
        {
            if (i > fadePanelTime) i = fadePanelTime;

            float fillAmount = i / fadePanelTime;

            winCanvasGroup.alpha = fillAmount;

            yield return null;
        }

        winCanvasGroup.alpha = 1;
    }

    private IEnumerator LevelChangeScreenAnim()
    {
        canPause = false;
        levelChangeCanvasGroup.alpha = 1;
        levelChangePlayerRTransform.anchoredPosition = levelChangePlayerDefaultPos;
        levelChangeCanvas.gameObject.SetActive(true);
        cursorVisibility = false;
        if (isOnKeyboard)
            Cursor.visible = cursorVisibility;
        Cursor.lockState = CursorLockMode.Confined;
        eventSystem.SetSelectedGameObject(null);

        yield return null;

        for (float i = 0; i <= fadePanelTime + Time.unscaledDeltaTime; i += Time.unscaledDeltaTime)
        {
            if (i > fadePanelTime) i = fadePanelTime;

            float fillAmount = i / fadePanelTime;

            levelChangeCanvasGroup.alpha = fillAmount;

            yield return null;
        }

        levelChangeCanvasGroup.alpha = 1;
        Time.timeScale = 1;
        levelChangePlayer.animator.SetInteger(Level1, levelChangeCurrentLvl);

    }

    public void LevelChangePlayerFinished()
    {
        if (levelChangeCurrentLvl < 4)
            LoadSceneFade(nextScene);
        else
        {
            if (victoryCanvasGroup)
            {
                StartCoroutine(ShowTotalVictoryScreen());
                cursorVisibility = true;
                if (isOnKeyboard)
                    Cursor.visible = cursorVisibility;
            }
            else
                StartCoroutine(LoadFade(false, -1, true));
        }
    }

    private IEnumerator ShowTotalVictoryScreen()
    {
        canPause = false;
        victoryCanvasGroup.alpha = 1;
        victoryCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        lastSelectedObj = victorySelectedObj;
        if (!isOnKeyboard)
            eventSystem.SetSelectedGameObject(lastSelectedObj);

        yield return null;

        for (float i = 0; i <= fadePanelTime + Time.unscaledDeltaTime; i += Time.unscaledDeltaTime)
        {
            if (i > fadePanelTime) i = fadePanelTime;

            float fillAmount = i / fadePanelTime;

            victoryCanvasGroup.alpha = fillAmount;

            yield return null;
        }

        victoryCanvasGroup.alpha = 1;
        winCanvas.gameObject.SetActive(false);
    }

    public void OpenSettings()
    {
        if (isMainGame)
            pauseCanvas.gameObject.SetActive(false);
        else
            mainMenuCanvas.gameObject.SetActive(false);

        BorderTypeSetting(false);
        settingsCanvas.gameObject.SetActive(true);
        lastSelectedObj = settingsMenuSelectedObj;
        if (!isOnKeyboard)
            eventSystem.SetSelectedGameObject(lastSelectedObj);
        Cursor.lockState = CursorLockMode.None;
        isInSettings = true;
    }

    public void CloseSettings()
    {
        settingsCanvas.gameObject.SetActive(false);

        if (isMainGame)
        {
            Cursor.lockState = CursorLockMode.Confined;
            pauseCanvas.gameObject.SetActive(true);
        }
        else
            mainMenuCanvas.gameObject.SetActive(true);

        lastSelectedObj = settingsMenuBackSelectedObj;
        if (!isOnKeyboard)
            eventSystem.SetSelectedGameObject(lastSelectedObj);
        isInSettings = false;
    }

    public void BorderTypeSetting(bool shouldRaise)
    {
        int screenType = PlayerPrefs.GetInt("ScreenType");

        if (shouldRaise)
        {
            screenType++;

            if (screenType >= 4) screenType = 0;

            PlayerPrefs.SetInt("ScreenType", screenType);
        }

        switch (screenType)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                settingsWindowTypeText.text = "Exclusive";
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                settingsWindowTypeText.text = "Fullscreen";
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
                settingsWindowTypeText.text = "Maximized";
                break;
            case 3:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                settingsWindowTypeText.text = "Windowed";
                break;
        }
    }

    public void UpdateVolumeSettings()
    {
        PlayerPrefs.SetFloat("sfxVolume", settingsSFXVolumeSlider.value);
        PlayerPrefs.SetFloat("musicVolume", settingsMusicVolumeSlider.value);

        audioManager.sfxAudioSource.volume = settingsSFXVolumeSlider.value;
        audioManager.musicAudioSource.volume = settingsMusicVolumeSlider.value;
    }

    private void SetVolumeSettings()
    {
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        float musicVolume = PlayerPrefs.GetFloat("musicVolume");

        settingsSFXVolumeSlider.value = sfxVolume;
        settingsMusicVolumeSlider.value = musicVolume;

        audioManager.sfxAudioSource.volume = sfxVolume;
        audioManager.musicAudioSource.volume = musicVolume;
    }

    public void UpdateHealthPotions(int potionCount)
    {
        healthPotionImage.color = potionCount > 0 ? Color.white : healthPotionsUnavailable;
        healthPotionText.text = potionCount > 0 ? potionCount.ToString() : string.Empty;
    }

    public void ContinueButton()
    {
        if (isMainGame)
        {
            abilityManager.SaveAbilities();
            player.SavePotions();
            StartCoroutine(LevelChangeScreenAnim());
        }
        else
        {
            StopCoroutine(fadeCoroutine);
            LoadSceneFade(nextScene);
        }
    }

    public void WinMainMenuButton()
    {
        mainMenuConfirmPanel.SetActive(true);
        lastSelectedObj = mainMenuConfirmSelectedObj;
        if (!isOnKeyboard)
            eventSystem.SetSelectedGameObject(lastSelectedObj);
    }

    public void WinMainMenuButtonCancel()
    {
        mainMenuConfirmPanel.SetActive(false);
        lastSelectedObj = mainMenuConfirmBackSelectedObj;
        if (!isOnKeyboard)
            eventSystem.SetSelectedGameObject(lastSelectedObj);
    }

    public void QuitButton()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        quitConfirmCanvas.gameObject.SetActive(true);
        lastSelectedObj = quitMenuSelectedObj;
        if (!isOnKeyboard)
            eventSystem.SetSelectedGameObject(lastSelectedObj);
    }

    public void CancelQuit()
    {
        mainMenuCanvas.gameObject.SetActive(true);
        quitConfirmCanvas.gameObject.SetActive(false);
        lastSelectedObj = quitMenuBackSelectedObj;
        if (!isOnKeyboard)
            eventSystem.SetSelectedGameObject(lastSelectedObj);
    }

    public void Exit()
    {
        StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(LoadFade(false, -1, true));
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public void OnKeyboardUsed()
    {
        if (isOnKeyboard) return;
        lastSelectedObj = eventSystem.currentSelectedGameObject;
        eventSystem.SetSelectedGameObject(null);
        isOnKeyboard = true;
        Cursor.visible = cursorVisibility;
    }

    public void OnControllerUsed()
    {
        if (!isOnKeyboard) return;
        eventSystem.SetSelectedGameObject(lastSelectedObj);
        isOnKeyboard = false;
        Cursor.visible = false;
    }

    private void ResetSave()
    {
        PlayerPrefs.DeleteAll();
        Debug.LogWarning("Removed all PlayerPrefs.");
        LoadSceneFade(0);
    }
}
