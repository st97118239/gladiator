using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public Player player;
    public AbilityManager abilityManager;
    public EventSystem eventSystem;
    public LevelManager levelManager;
    public EnemyManager enemyManager;
    public Image fadePanel;
    public Canvas abilityCanvas;
    public GameObject abilityMenuSelectedObj;
    public Canvas pauseCanvas;
    public GameObject pauseMenuSelectedObj;
    public Canvas settingsCanvas;
    public GameObject settingsMenuSelectedObj;
    public GameObject settingsMenuBackSelectedObj;
    public Canvas mainMenuCanvas;
    public GameObject mainMenuSelectedObj;
    public Canvas quitConfirmCanvas;
    public GameObject quitMenuSelectedObj;
    public GameObject quitMenuBackSelectedObj;
    public bool isMainGame;

    public AbilitySlot[] abilitySlots;
    [SerializeField] private AbilityCard[] abilityCards;
    [SerializeField] private Button confirmButton;

    [SerializeField] private float fadePanelTime;

    private bool isPaused;
    private bool hasChosenAbility;

    private void Start()
    {
        fadePanel.color = Color.gray2;
        Time.timeScale = 1;
        StartCoroutine(LoadFade(true, -1, false));

        if (!isMainGame)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            eventSystem.SetSelectedGameObject(mainMenuSelectedObj);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void ShowAbilityMenu()
    {
        player.canAttack = false;
        player.movementScript.canMove = false;

        hasChosenAbility = false;

        List<Ability> possibleAbilities = new();

        foreach (AbilitySort sort in levelManager.enemyManager.currentWave.abilitySortsToRoll)
        {
            switch (sort)
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
                    if (abilityManager.secondary == -1)
                        possibleAbilities.AddRange(abilityManager.secondaries);
                    if (abilityManager.ability1 == -1 || abilityManager.ability2 == -1)
                        possibleAbilities.AddRange(abilityManager.powers);
                    if (abilityManager.passivesUnlocked < abilityManager.passives.Length)
                        possibleAbilities.AddRange(abilityManager.passives);
                    break;
            }
        }

        foreach (AbilityCard card in abilityCards)
        {
            int idx = Random.Range(0, possibleAbilities.Count);

            card.Load(possibleAbilities[idx]);
            possibleAbilities.RemoveAt(idx);
        }

        abilityCanvas.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(abilityMenuSelectedObj);
        abilityMenuSelectedObj.GetComponent<UIButton>().OnSelect(null);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
        eventSystem.SetSelectedGameObject(null);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        foreach (AbilityCard card in abilityCards)
        {
            if (card.isSelected)
                abilityManager.NewPower(card.ability);

            card.Reset();
        }

        player.canAttack = true;
        player.movementScript.canMove = true;
    }

    public void NewAbility(Ability givenAbility, int idx)
    {
        abilitySlots[idx].Activate(givenAbility);

        levelManager.WaveEnd();
    }

    public void PauseMenu()
    {
        if (fadePanel.IsActive())
            fadePanel.gameObject.SetActive(false);

        pauseCanvas.gameObject.SetActive(!isPaused);
        player.canAttack = isPaused;
        player.movementScript.canMove = isPaused;
        Time.timeScale = isPaused ? 1 : 0;
        eventSystem.SetSelectedGameObject(isPaused ? null : pauseMenuSelectedObj);
        Cursor.visible = !isPaused;
        Cursor.lockState = !isPaused ? CursorLockMode.Confined : CursorLockMode.None;

        isPaused = !isPaused;
    }

    public void Restart(int sceneToLoad)
    {
        StartCoroutine(LoadFade(false, sceneToLoad, false));
    }

    private IEnumerator LoadFade(bool shouldReverse, int sceneToLoad, bool shouldQuit)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
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
            fadePanel.gameObject.SetActive(false);

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
        StartCoroutine(LoadFade(false, sceneToLoad, false));
    }

    public void OpenSettings()
    {
        if (isMainGame)
            pauseCanvas.gameObject.SetActive(false);
        else
            mainMenuCanvas.gameObject.SetActive(false);

        settingsCanvas.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(settingsMenuSelectedObj);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseSettings()
    {
        settingsCanvas.gameObject.SetActive(false);

        if (isMainGame)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            pauseCanvas.gameObject.SetActive(true);
        }
        else
            mainMenuCanvas.gameObject.SetActive(true);

        eventSystem.SetSelectedGameObject(settingsMenuBackSelectedObj);
    }

    public void QuitButton()
    {
        mainMenuCanvas.gameObject.SetActive(false);
        quitConfirmCanvas.gameObject.SetActive(true);
        eventSystem.SetSelectedGameObject(quitMenuSelectedObj);
    }

    public void CancelQuit()
    {
        mainMenuCanvas.gameObject.SetActive(true);
        quitConfirmCanvas.gameObject.SetActive(false);
        eventSystem.SetSelectedGameObject(quitMenuBackSelectedObj);
    }

    public void Exit()
    {
        StartCoroutine(LoadFade(false, -1, true));
    }

    public void Quit()
    {
        Debug.Log("Quiting game.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
