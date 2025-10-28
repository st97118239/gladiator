using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackButton : MonoBehaviour, ICancelHandler
{
    [SerializeField] private Button.ButtonClickedEvent onBackKey = new();

    public void OnCancel(BaseEventData eventData)
    {
        onBackKey?.Invoke();
    }
}
