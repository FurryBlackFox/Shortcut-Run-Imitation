using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIClaimPointsButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText = default;
    
    [Header("If Bonus Button")]
    [SerializeField] private bool hasBonus = true;
    [SerializeField] private TextMeshProUGUI extraPointsMultText = default;
    
    public UnityEventIntBool onClaimPoints;
    
    private GameSettings gameSettings;
    
    public int PointsCount { get; private set; }
    private void Awake()
    {
        #if UNITY_EDITOR

        CustomTools.IsNull(pointsText, nameof(pointsText), name);
        
        if(hasBonus)
            CustomTools.IsNull(pointsText, nameof(pointsText), name);

        #endif
        gameSettings = GameDataKeeper.S.GameSettings;
    }
    
    private void OnEnable()
    {
        VictoryPointsCounter.OnCharacterFinish += SetPlayerPoints;
        //Finish.OnPlayerFinish += PlayerFinishHandler;
    }

    private void OnDisable()
    {
        VictoryPointsCounter.OnCharacterFinish += SetPlayerPoints;
        //Finish.OnPlayerFinish -= PlayerFinishHandler;
    }
    
    
    private void PlayerFinishHandler()
    {

        
    }
    
    private void SetPlayerPoints(Character character, int points)
    {

        if(!(character is Player))
            return;
        
        PointsCount = hasBonus ? points * gameSettings.ExtraPointsMultiplier : points;
        pointsText.SetText($"{PointsCount}"); 
        
        if(hasBonus)
            extraPointsMultText.SetText($"x{gameSettings.ExtraPointsMultiplier}");
    }

    
    public void ClaimPoints()
    {
        onClaimPoints?.Invoke(PointsCount, hasBonus);
    }

    
}
