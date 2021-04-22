using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResultPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI characterNameText = default;
    public TextMeshProUGUI CharacterNameText => characterNameText;
    [SerializeField] private TextMeshProUGUI pointsText = default;
    public TextMeshProUGUI PointsText => pointsText;
    [SerializeField] private Image backgroundImage = default;
    public Image BackgroundImage => backgroundImage;
    
    private void Awake()
    {
        #if UNITY_EDITOR

        CustomTools.IsNull(characterNameText, nameof(characterNameText), name);
        CustomTools.IsNull(pointsText, nameof(pointsText), name);
        CustomTools.IsNull(backgroundImage, nameof(backgroundImage), name);
        
        #endif
    }
}
