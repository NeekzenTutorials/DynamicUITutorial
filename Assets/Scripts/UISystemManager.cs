using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;


public class UISystemManager : MonoBehaviour
{
    public List<UIElement> uiElements = new List<UIElement>();
    private Vector2 screenSize;
    private Vector2 initialScreenSize;

    private void Awake()
    {
        screenSize = GetScreenSize();
        initialScreenSize = screenSize;
        ValidateUIElements();
        InitializeCanvasScaler();
        UpdateUIElementsLayout();
    }

    //private void OnValidate()
    //{
    //        UpdateUIElementsLayout();
    //}

    private Vector2 GetScreenSize() => new Vector2(Screen.width, Screen.height);

    private void ValidateUIElements()
    {
        foreach (var uiElement in uiElements)
        {
            if (!uiElement.element.CompareTag("UI"))
            {
                throw new InvalidOperationException($"Element '{uiElement.element.name}' is not an UI object (doesn't have an UI tag)");
            }
        }
    }

    private void InitializeCanvasScaler()
    {
        var canvasScaler = GetComponentInChildren<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasScaler.referenceResolution = screenSize;
        }
    }

    private void UpdateUIElementsLayout()
    {
         var canvasScaler = GetComponentInChildren<CanvasScaler>();
        foreach (var uiElement in uiElements)
        {
            uiElement.UpdateLayout(screenSize, canvasScaler, initialScreenSize);
        }
    }

     public void SetResolutionToFullHD()
    {
        StartCoroutine(ChangeResolutionAndDisplay(1920, 1080));
    }

    public void SetResolutionTo1400x1050()
    {
        StartCoroutine(ChangeResolutionAndDisplay(1400, 1050));
    }

    public void SetResolutionTo1600x900()
    {
        StartCoroutine(ChangeResolutionAndDisplay(1600, 900));
    }   

    public void SetResolutionTo800x600()
    {
        StartCoroutine(ChangeResolutionAndDisplay(800, 600));
    }

    public void SetResolutionTo1152x1864()
    {
        StartCoroutine(ChangeResolutionAndDisplay(1152, 864));
    }

    private IEnumerator ChangeResolutionAndDisplay(int width, int height)
    {

        Canvas childCanvas = GetComponentInChildren<Canvas>();

        if (childCanvas != null)
        {
            CanvasScaler canvasScaler = childCanvas.GetComponent<CanvasScaler>();

            if (canvasScaler != null)
            {
                canvasScaler.referenceResolution = new Vector2(width, height);
            }
        }

        Screen.SetResolution(width, height, false);

        yield return new WaitForSeconds(0.01f);

        initialScreenSize = screenSize;
        screenSize = new Vector2(Screen.width, Screen.height);
        UpdateUIElementsLayout();
    }
}

[Serializable]
public class UIElement
{
    public GameObject element;
    [Range(-50, 50)]
    public float horizontalPosition;
    [Range(-50, 50)]
    public float verticalPosition;
    [Range(0, 100)]
    public float widthPercentage;
    [Range(0, 100)]
    public float heightPercentage;

    private const int minFontSize = 10; // Minimum font size
    private const int maxFontSize = 78; // Maximum font size

    public void UpdateLayout(Vector2 screenSize, CanvasScaler canvasScaler, Vector2 initialScreenSize)
    {
        var rectTransform = element.GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogWarning($"GameObject '{element.name}' does not have a RectTransform.");
            return;
        }

        UpdatePositionAndSize(rectTransform, screenSize);
        UpdateTextComponents(screenSize, canvasScaler, initialScreenSize);
    }

    private void UpdatePositionAndSize(RectTransform rectTransform, Vector2 screenSize)
    {
        rectTransform.anchoredPosition = new Vector2(screenSize.x * horizontalPosition / 100f, screenSize.y * verticalPosition / 100f);
        rectTransform.sizeDelta = new Vector2(screenSize.x * widthPercentage / 100f, screenSize.y * heightPercentage / 100f);
    }

    private void UpdateTextComponents(Vector2 screenSize, CanvasScaler canvasScaler, Vector2 initialScreenSize)
    {
        UpdateTextFontSize(element.GetComponentsInChildren<Text>(), screenSize, canvasScaler, initialScreenSize);
        UpdateTextMeshProFontSize(element.GetComponentsInChildren<TextMeshProUGUI>(), screenSize, canvasScaler, initialScreenSize);
    }

    private void UpdateTextFontSize(Text[] textComponents, Vector2 screenSize, CanvasScaler canvasScaler, Vector2 initialScreenSize)
    {
        foreach (var textComponent in textComponents)
        {
            textComponent.fontSize = CalculateNewFontSize(textComponent.fontSize, screenSize, canvasScaler, initialScreenSize);
        }
    }

    private void UpdateTextMeshProFontSize(TextMeshProUGUI[] textMeshProComponents, Vector2 screenSize, CanvasScaler canvasScaler, Vector2 initialScreenSize)
    {
        foreach (var textMeshProComponent in textMeshProComponents)
        {
            textMeshProComponent.fontSize = CalculateNewFontSize((int)textMeshProComponent.fontSize, screenSize, canvasScaler, initialScreenSize);
        }
    }

    private int CalculateNewFontSize(int originalFontSize, Vector2 screenSize, CanvasScaler canvasScaler, Vector2 initialScreenSize)
    {
        Vector2 referenceResolution = new Vector2(1920, 1080);

        float widthRatio = screenSize.x / initialScreenSize.x;
        float heightRatio = screenSize.y / initialScreenSize.y;
        float scaleRatio = Mathf.Sqrt(widthRatio * heightRatio);

        int newFontSize = Mathf.RoundToInt(originalFontSize * scaleRatio);

        // Limiter l'augmentation/diminution de la taille de la police
        // TODO - Faire en sorte que lorsqu'on démarre le jeu, les fonts soit automatiquement calculé en fonction par rapport à la fenêtre de référence
        float maxChangeFactor = 2.75f; // Exemple: 150%
        newFontSize = Mathf.Clamp(newFontSize, Mathf.RoundToInt(originalFontSize / maxChangeFactor), Mathf.RoundToInt(originalFontSize * maxChangeFactor));

        return newFontSize;
    }
}
