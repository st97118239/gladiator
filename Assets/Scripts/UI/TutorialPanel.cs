using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialPanel : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private TMP_Text text;

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
        eventSystem.SetSelectedGameObject(gameObject);
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