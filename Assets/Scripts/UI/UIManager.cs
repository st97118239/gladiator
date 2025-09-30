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
    public EventSystem eventSystem;
    public LevelManager levelManager;
    public EnemyManager enemyManager;
    public Image healthPotionImage;
    public TMP_Text healthPotionText;
    public Color healthPotionsUnavailable;
    public Image fadePanel;
    public Canvas abilityCanvas;
    public CanvasGroup abilityCanvasGroup;
    public GameObject abilityMenuSelectedObj;
    public Canvas pauseCanvas;
    public GameObject pauseMenuSelectedObj;
    public Canvas settingsCanvas;
    public GameObject settingsMenuSelectedObj;
    public GameObject settingsMenuBackSelectedObj;
    public Canvas deathCanvas;
    public CanvasGroup deathCanvasGroup;
    public GameObject deathSelectedObj;
    public Canvas winCanvas;
    public CanvasGroup winCanvasGroup;
    public GameObject winSelectedObj;
    public GameObject mainMenuConfirmPanel;
    public GameObject mainMenuConfirmSelectedObj;
    public GameObject mainMenuConfirmBackSelectedObj;
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
        player.hasAttackPreview = false;
        player.movementScript.canMove = false;
        player.canHeal = false;
        player.abilityManager.canUseSecondary = false;
        player.abilityManager.canUsePowers = false;

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
                        possibleAbilities.AddRange(abilityManager.secondaries);
                        possibleAbilities.AddRange(abilityManager.powers);
                        possibleAbilities.AddRange(abilityManager.passives);
                    break;
            }
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

        // TO-DO: if less than 3 possible abilities, make sure less than 3 cards show!

        int idx;

        switch (possibleAbilities.Count)
        {
            case 0:
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
        eventSystem.SetSelectedGameObject(abilityMenuSelectedObj);
        abilityMenuSelectedObj.GetComponent<UIButton>().OnSelect(null);
        Cursor.visible = true;
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
        player.hasAttackPreview = true;
        player.movementScript.canMove = true;
        player.canHeal = true;
        player.abilityManager.canUseSecondary = true;
        player.abilityManager.canUsePowers = true;
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

        isPaused = !isPaused;

        pauseCanvas.gameObject.SetActive(isPaused);
        player.canAttack = !isPaused;
        player.movementScript.canMove = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        eventSystem.SetSelectedGameObject(isPaused ? pauseMenuSelectedObj : null);
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Confined;
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

    public void ShowDeathScreen()
    {
        Time.timeScale = 0;
        StartCoroutine(DeathScreenAnim());
    }

    public void ShowWinScreen()
    {
        Time.timeScale = 0;
        StartCoroutine(WinScreenAnim());
    }

    private IEnumerator DeathScreenAnim()
    {
        deathCanvasGroup.alpha = 1;
        deathCanvas.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        eventSystem.SetSelectedGameObject(deathSelectedObj);

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
        winCanvasGroup.alpha = 1;
        winCanvas.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        eventSystem.SetSelectedGameObject(winSelectedObj);

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

    public void UpdateHealthPotions(int potionCount)
    {
        healthPotionImage.color = potionCount > 0 ? Color.white : healthPotionsUnavailable;
        healthPotionText.text = potionCount > 0 ? potionCount.ToString() : string.Empty;
    }

    public void ContinueButton()
    {
        Debug.Log("Fuck you");
        Exit();
    }

    public void WinMainMenuButton()
    {
        mainMenuConfirmPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(mainMenuConfirmSelectedObj);
    }

    public void WinMainMenuButtonCancel()
    {
        mainMenuConfirmPanel.SetActive(false);
        eventSystem.SetSelectedGameObject(mainMenuConfirmBackSelectedObj);
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
