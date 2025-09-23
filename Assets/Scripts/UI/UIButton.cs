using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool isSelected;

    public Color defaultColor;
    public Color defaultTextColor;
    public Color hoverColor;
    public Color hoverTextColor;
    public Vector3 defaultScale;
    public Vector3 selectScale;
    public float fadeTime;
    public float scaleTime;

    public Image image;
    public Image image2;
    public Image image3;
    public TMP_Text text;

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
        transform.localScale = defaultScale;
        image.color = defaultColor;
        image2.color = defaultColor;
        image3.color = defaultColor;
        text.color = defaultTextColor;
    }

    public void Deselect()
    {
        if (!isSelected) return;

        StartCoroutine(Scale(true));
        StartCoroutine(Fade(true));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected) return;

        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
                onLeftClick?.Invoke();
                StartCoroutine(Scale(false));
                break;
            case PointerEventData.InputButton.Right:
                onRightClick?.Invoke();
                StartCoroutine(Scale(false));
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return;

        onHoverEnter?.Invoke();
        StartCoroutine(Fade(false));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;

        onHoverExit?.Invoke();
        StartCoroutine(Fade(true));
    }

    private IEnumerator Fade(bool shouldReverse)
    {
        yield return null;

        Color startColor = shouldReverse ? hoverColor : defaultColor;
        Color endColor = shouldReverse ? defaultColor : hoverColor;
        Color startTextColor = shouldReverse ? hoverTextColor : defaultTextColor;
        Color endTextColor = shouldReverse ? defaultTextColor : hoverTextColor;

        for (float i = 0; i <= fadeTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > fadeTime) i = fadeTime;

            float fillAmount = i / fadeTime;

            image.color = Color.Lerp(startColor, endColor, fillAmount);
            image2.color = Color.Lerp(startColor, endColor, fillAmount);
            image3.color = Color.Lerp(startColor, endColor, fillAmount);
            text.color = Color.Lerp(startTextColor, endTextColor, fillAmount);

            yield return null;
        }
    }

    private IEnumerator Scale(bool shouldReverse)
    {
        isSelected = !shouldReverse;

        yield return null;

        Vector3 startScale = shouldReverse ? selectScale : defaultScale;
        Vector3 endScale = shouldReverse ? defaultScale : selectScale;

        for (float i = 0; i <= scaleTime + Time.deltaTime; i += Time.deltaTime)
        {
            if (i > scaleTime) i = scaleTime;

            float scaleAmount = i / scaleTime;

            image.transform.localScale = Vector3.Lerp(startScale, endScale, scaleAmount);

            yield return null;
        }
    }
}
