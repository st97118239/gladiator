using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UIManager : MonoBehaviour
{
    public Player player;
    public AbilityManager abilityManager;
    public LevelManager levelManager;
    public EnemyManager enemyManager;
    public Image fadePanel;
    public Canvas abilityCanvas;
    public Canvas pauseCanvas;
    public InputAction pauseAction;

    [SerializeField] private AbilitySlot[] abilitySlots;
    [SerializeField] private AbilityCard[] abilityCards;
    [SerializeField] private Button confirmButton;

    [SerializeField] private float fadePanelTime;

    private bool isPaused;

    private void Start()
    {
        fadePanel.color = Color.gray2;
        pauseAction.Enable();
        pauseAction.started += ctx => { PauseMenu(isPaused); };
        Time.timeScale = 1;
        StartCoroutine(LoadFade(true, -1));
    }

    public void ShowAbilityMenu()
    {
        confirmButton.interactable = false;

        List<Ability> possibleAbilities = new();

        foreach (AbilitySort sort in levelManager.enemyManger.currentWave.abilitySortsToRoll)
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
                    if (!abilityManager.secondary)
                        possibleAbilities.AddRange(abilityManager.secondaries);
                    if (!abilityManager.ability1 || !abilityManager.ability2)
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
    }

    public void DifferentAbilitySelected()
    {
        foreach (AbilityCard card in abilityCards)
        {
            card.Deselect();
        }

        confirmButton.interactable = true;
    }

    public void ConfirmAbility()
    {
        abilityCanvas.gameObject.SetActive(false);

        foreach (AbilityCard card in abilityCards)
        {
            if (card.isSelected)
                abilityManager.NewPower(card.ability);

            card.Reset();
        }
    }

    public void NewAbility(Ability givenAbility, int idx)
    {
        abilitySlots[idx].Activate(givenAbility);

        levelManager.WaveEnd();
    }

    public void PauseMenu(bool toggle)
    {
        if (fadePanel.IsActive())
            fadePanel.gameObject.SetActive(false);

        pauseCanvas.gameObject.SetActive(!toggle);
        isPaused = !toggle;
        player.canAttack = toggle;
        player.movementScript.canMove = toggle;
        Time.timeScale = toggle ? 1 : 0;
    }

    public void Restart(int sceneToLoad)
    {
        pauseAction.Dispose();
        StartCoroutine(LoadFade(false, sceneToLoad));
    }

    private IEnumerator LoadFade(bool shouldReverse, int sceneToLoad)
    {
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
        else
        {
            yield return new WaitForSecondsRealtime(1f);
            LoadScene(sceneToLoad);
        }
    }

    private static void LoadScene(int sceneToLoad)
    {
        SceneManager.LoadSceneAsync(sceneToLoad);
    }
}
