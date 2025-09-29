using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler, ISelectHandler, IDeselectHandler
{
    public bool isSelected;

    public Color defaultColor = Color.white;
    public Color defaultTextColor = Color.black;
    public Color hoverColor = Color.white;
    public Color hoverTextColor = Color.black;
    public Vector3 defaultScale = Vector3.one;
    public Vector3 selectScale = Vector3.one;
    public float fadeTime = 0.3f;
    public float scaleTime;

    public Image image1;
    public Image image2;
    public Image image3;
    public TMP_Text text;

    public bool hasImage1;
    public bool hasImage2;
    public bool hasImage3;
    public bool hasText;
    public bool shouldFade;
    public bool shouldScale;
    public bool lmbScale;
    public bool rmbScale;

    [SerializeField] private Button.ButtonClickedEvent onLeftClick = new();
    [SerializeField] private Button.ButtonClickedEvent onRightClick = new();
    [SerializeField] private Button.ButtonClickedEvent onHoverEnter = new();
    [SerializeField] private Button.ButtonClickedEvent onHoverExit = new();

    private WaitForSeconds fadeDelay;
    private WaitForSeconds scaleDelay;

    private void Awake()
    {
        fadeDelay = new WaitForSeconds(fadeTime);
        scaleDelay = new WaitForSeconds(scaleTime);
    }

    public Button.ButtonClickedEvent OnLeftClick
    {
        get => onLeftClick;
        set => onLeftClick = value;
    }

    public Button.ButtonClickedEvent OnRightClick
    {
        get => onRightClick;
        set => onRightClick = value;
    }

    public void Reset()
    {
        isSelected = false;
        if (shouldScale)
            transform.localScale = defaultScale;
        if (hasImage1)
            image1.color = defaultColor;
        if (hasImage2)
            image2.color = defaultColor;
        if (hasImage3)
            image3.color = defaultColor;
        if (hasText)
            text.color = defaultTextColor;
    }

    public void Deselect()
    {
        if (!isSelected) return;

        if (shouldScale)
            StartCoroutine(Scale(true));
        if (shouldFade)
            StartCoroutine(Fade(true));
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (isSelected) return;

        onHoverEnter?.Invoke();
        if (shouldFade)
            StartCoroutine(Fade(false));
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (isSelected) return;

        onHoverExit?.Invoke();
        if (shouldFade)
            StartCoroutine(Fade(true));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected) return;

        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                onLeftClick?.Invoke();
                if (shouldScale && lmbScale)
                    StartCoroutine(Scale(false));
                break;
            case PointerEventData.InputButton.Right:
                onRightClick?.Invoke();
                if (shouldScale && rmbScale)
                    StartCoroutine(Scale(false));
                break;
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (isSelected) return;

        onLeftClick?.Invoke();
        if (shouldScale && lmbScale)
            StartCoroutine(Scale(false));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return;

        onHoverEnter?.Invoke();
        if (shouldFade)
            StartCoroutine(Fade(false));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;

        onHoverExit?.Invoke();
        if (shouldFade)
            StartCoroutine(Fade(true));
    }

    private IEnumerator Fade(bool shouldReverse)
    {
        if (!shouldFade) yield break;

        yield return null;

        Color endColor = shouldReverse ? defaultColor : hoverColor;
        Color endTextColor = shouldReverse ? defaultTextColor : hoverTextColor;

        for (float i = 0; i <= fadeTime + Time.unscaledDeltaTime; i += Time.unscaledDeltaTime)
        {
            if (i > fadeTime) i = fadeTime;

            float fillAmount = i / fadeTime;

            Color imageColor = Color.Lerp(image1.color, endColor, fillAmount);

            if (hasImage1)
                image1.color = imageColor;
            if (hasImage2)
                image2.color = imageColor;
            if (hasImage3)
                image3.color = imageColor;
            if (hasText)
                text.color = Color.Lerp(text.color, endTextColor, fillAmount);

            yield return null;
        }
    }

    private IEnumerator Scale(bool shouldReverse)
    {
        if (!shouldScale) yield break;

        isSelected = !shouldReverse;

        yield return null;

        Vector3 endScale = shouldReverse ? defaultScale : selectScale;

        for (float i = 0; i <= scaleTime + Time.unscaledDeltaTime; i += Time.unscaledDeltaTime)
        {
            if (i > scaleTime) i = scaleTime;

            float scaleAmount = i / scaleTime;

            if (hasImage1)
                image1.transform.localScale = Vector3.Lerp(image1.transform.localScale, endScale, scaleAmount);

            yield return null;
        }
    }
}
