using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonHoverDetection : MonoBehaviour, ISelectHandler, ISubmitHandler
{
    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    public void OnSelect(BaseEventData eventData)
    {
       audioManager.PlayButtonHover();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        audioManager.PlayButtonSelect();
    }
}
