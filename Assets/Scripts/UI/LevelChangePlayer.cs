using UnityEngine;

public class LevelChangePlayer : MonoBehaviour
{
    public bool canContinue;
    public bool hasContinued;

    public Animator animator;

    [SerializeField] private UIManager uiManager;

    private void Update()
    {
       if (canContinue && !hasContinued)
           Continue();
    }

    public void Continue()
    {
        hasContinued = true;
        uiManager.LevelChangePlayerFinished();
    }
}
