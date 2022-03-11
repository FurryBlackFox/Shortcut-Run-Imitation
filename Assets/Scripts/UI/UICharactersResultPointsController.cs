using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharactersResultPointsController : MonoBehaviour
{
    [SerializeField] private List<UIResultPanel> UIResultPanels = default;  
    [SerializeField] private Color playerColor = Color.green;
    [SerializeField, Range(1f, 2f)] private float playerMultiplier = 1.2f;
    [SerializeField, Range(1f, 2f)] private float paddingModifier = 1.4f;
    [SerializeField] private bool hideUIOnStart = true;

    private int currentFinishPosition;
    private bool playerFinished = false;
    private void Awake()
    {
        #if UNITY_EDITOR
        
        if(UIResultPanels.Count == 0)
            Debug.LogError($"{nameof(UIResultPanels)} in {name} are not assigned"); 
        
        #endif
    }

    private void Start()
    {
        if(hideUIOnStart)
            foreach (var resultPanel in UIResultPanels)
            {
                resultPanel.IsVisible = false;
            }
    }

    private void OnEnable()
    {
        VictoryPointsCounter.OnCharacterFinish += SetResult;
        Finish.OnPlayerFinish += ShowResults;
    }

    private void OnDisable()
    {
        VictoryPointsCounter.OnCharacterFinish -= SetResult;
        Finish.OnPlayerFinish -= ShowResults;
    }

        
    private void SetResult(Character character, int points)
    {
        var panel = UIResultPanels[currentFinishPosition];
        if (playerFinished)
            panel.IsVisible = true;
        panel.CharacterNameText.SetText(character.CharacterName);
        panel.PointsText.SetText($"{points}");
        currentFinishPosition++;
    }


    private void ShowResults()
    {
        playerFinished = true;
        
        for (var i = 0; i < currentFinishPosition; i++)
        {
            UIResultPanels[i].IsVisible = true;
        }
        
        SetPlayerPanel();
        //CalculatePadding();
    }

    private void SetPlayerPanel()
    {
        var position = currentFinishPosition;
        var rectTransform = UIResultPanels[position].GetComponent<RectTransform>();
        rectTransform.localScale *= playerMultiplier;
        var pos = rectTransform.anchoredPosition3D;
        pos *= playerMultiplier;
        rectTransform.anchoredPosition = pos;

        UIResultPanels[position].BackgroundImage.color = playerColor;
        UIResultPanels[position].CharacterNameText.color = playerColor;
    }
    
    
    private void CalculatePadding()
    {
        var prevOffset = 0f;
        foreach (var panel in UIResultPanels)
        {
            var rectTransform = panel.GetComponent<RectTransform>();
            var pos = rectTransform.anchoredPosition;
            var height = rectTransform.rect.height * rectTransform.localScale.y;
            pos.y = prevOffset - height * 0.5f;
            prevOffset -= height * paddingModifier;
            rectTransform.anchoredPosition = pos;
        }
    }


}
