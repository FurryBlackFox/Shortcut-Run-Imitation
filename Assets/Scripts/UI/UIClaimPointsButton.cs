using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIClaimPointsButton : MonoBehaviour
{
    public static event Action<int, bool> OnClaimPoints; 
    [SerializeField] private TextMeshProUGUI pointsText = default;
    
    [Header("If Bonus Button")]
    [SerializeField] private bool hasBonus = true;
    [SerializeField] private TextMeshProUGUI extraPointsMultText = default;

    private bool playerFinished;
    private GameSettings gameSettings;

    private int pointsCount;
    public int PointsCount => pointsCount;
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
        Finish.OnPlayerFinish += PlayerFinishHandler;
    }

    private void OnDisable()
    {
        VictoryPointsCounter.OnCharacterFinish += SetPlayerPoints;
        Finish.OnPlayerFinish -= PlayerFinishHandler;
    }
    
    
    private void PlayerFinishHandler()
    {
        playerFinished = true;
    }
    
    private void SetPlayerPoints(Character character, int points)
    {
        if(!playerFinished)
            return;
        
        pointsCount = hasBonus ? points * gameSettings.ExtraPointsMultiplier : points;
        pointsText.SetText($"{pointsCount}"); 
        
        if(hasBonus)
            extraPointsMultText.SetText($"x{gameSettings.ExtraPointsMultiplier}");
    }

    public void OnButtonClick()
    {
        OnClaimPoints?.Invoke(pointsCount, hasBonus);
    }
    
}
