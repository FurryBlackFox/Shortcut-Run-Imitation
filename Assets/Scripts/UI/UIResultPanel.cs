using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIResultPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI characterNameText = default;
    public TextMeshProUGUI CharacterNameText => characterNameText;
    [SerializeField] private TextMeshProUGUI pointsText = default;
    public TextMeshProUGUI PointsText => pointsText;
    [SerializeField] private Image backgroundImage = default;
    public Image BackgroundImage => backgroundImage;

    private CanvasGroup canvasGroup = default;

    private bool isVisible = false;
    
    public bool IsVisible
    {
        get => isVisible;
        set
        {
            isVisible = value;
            canvasGroup.alpha = isVisible ? 1f : 0f;
        }
    }
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

#if UNITY_EDITOR

        CustomTools.IsNull(characterNameText, nameof(characterNameText), name);
        CustomTools.IsNull(pointsText, nameof(pointsText), name);
        CustomTools.IsNull(backgroundImage, nameof(backgroundImage), name);
        
        #endif
    }
}
