using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button button;
    [SerializeField] private float timeUntilEnabled;

    private int id;
    private TutorialInfo nextPanel;
    private bool shouldDisableId;

    public void Pressed()
    {
        if (shouldDisableId)
            PlayerPrefs.SetInt("Tutorial" + id, 1);

        if (nextPanel)
            NewInfo(nextPanel);
        else
            Continue();
    }

    public void NewInfo(TutorialInfo givenInfo)
    {
        Time.timeScale = 0;
        levelManager.uiManager.canPause = false;
        levelManager.player.movementScript.canMove = false;
        levelManager.player.canAttack = false;
        levelManager.player.hasAttackPreview = false;
        levelManager.player.canHeal = false;
        levelManager.player.abilityManager.canUseSecondary = false;
        levelManager.player.abilityManager.canUsePowers = false;
        id = givenInfo.id;
        text.text = givenInfo.text;
        nextPanel = givenInfo.nextPanel;
        shouldDisableId = !givenInfo.alwaysShow;
        gameObject.SetActive(true);
        button.interactable = false;
        eventSystem.SetSelectedGameObject(gameObject);
        StartCoroutine(EnableButton());
    }

    private IEnumerator EnableButton()
    {
        for (float i = 0; i < timeUntilEnabled; i += Time.unscaledDeltaTime)
        {
            yield return null;
        }

        button.interactable = true;
    }

    private void Continue()
    {
        eventSystem.SetSelectedGameObject(null);
        gameObject.SetActive(false);
        Time.timeScale = 1;
        levelManager.uiManager.canPause = true;
        levelManager.player.movementScript.canMove = true;
        levelManager.player.canAttack = true;
        levelManager.player.hasAttackPreview = true;
        levelManager.player.canHeal = true;
        levelManager.player.abilityManager.canUseSecondary = true;
        levelManager.player.abilityManager.canUsePowers = true;
    }
}