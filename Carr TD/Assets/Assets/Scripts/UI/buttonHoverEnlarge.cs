using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class buttonHoverEnlarge : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float targetScale = 1.2f;  // Maximum scale when hovered
    public float transitionSpeed = 5f; // Speed of enlargement/shrinking

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector3 targetScaleVector;
    private bool isHovered = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        targetScaleVector = originalScale * targetScale;
    }

    void Update()
    {
        // Smoothly transition to the target scale based on whether it's hovered or not
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, isHovered ? targetScaleVector : originalScale, Time.deltaTime * transitionSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true; // When hovered, start enlarging
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false; // When not hovered, shrink back
    }
}
