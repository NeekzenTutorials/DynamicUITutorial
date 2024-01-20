using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

[ExecuteInEditMode]
public class UISystemManager : MonoBehaviour
{
    public List<UIElement> uiElements = new List<UIElement>();
    private Vector2 screenSize;

    private void Awake()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        UIListCheck();
        DisplayUIElement();
    }

    private void OnValidate()
    {
        DisplayUIElement();
    }

    private void DisplayUIElement()
    {
        foreach (UIElement uiElement in uiElements)
        {
            GameObject element = uiElement.element;
            RectTransform uiRectTransform = element.GetComponent<RectTransform>();

            if (uiRectTransform != null)
            {
                float horizontalPosition = screenSize.x * uiElement.horizontalPosition / 100f;
                float verticalPosition = screenSize.y * uiElement.verticalPosition / 100f;

                uiRectTransform.anchoredPosition = new Vector2(horizontalPosition, verticalPosition);
            }
            else
            {
                Debug.LogWarning("Le GameObject " + element.name + " n'a pas de RectTransform.");
            }
        }
    }

    private void UIListCheck()
    {
        foreach (UIElement uiElement in uiElements)
        {
            if (!uiElement.element.CompareTag("UI"))
            {
                throw new InvalidOperationException("Element is not an UI object (doesn't have an UI tag)");
            }
        }
    }

    public void SetResolutionToFullHD()
    {
        StartCoroutine(ChangeResolutionAndDisplay(1920, 1080));
    }

    public void SetResolutionTo4K()
    {
        StartCoroutine(ChangeResolutionAndDisplay(1200, 800));
    }

    private IEnumerator ChangeResolutionAndDisplay(int width, int height)
    {
        Screen.SetResolution(width, height, false);

        yield return new WaitForSeconds(0.01f);

        screenSize = new Vector2(Screen.width, Screen.height);
        DisplayUIElement();
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
}
