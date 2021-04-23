using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScreensController : MonoBehaviour
{
    [SerializeField] private CanvasGroup gameOverScreen = default;
    [SerializeField] private CanvasGroup finishScreen = default;
    [SerializeField] private CanvasGroup playtimeScreen = default;
    [SerializeField] private CanvasGroup menuScreen = default;
    [SerializeField] private CanvasGroup settingsScreen = default;
    [SerializeField] private CanvasGroup onHomeScreen = default;

    private void Awake()
    {
        #if UNITY_EDITOR
                
        CustomTools.IsNull(gameOverScreen, nameof(gameOverScreen), name);
        CustomTools.IsNull(finishScreen, nameof(finishScreen), name);
        CustomTools.IsNull(playtimeScreen, nameof(playtimeScreen), name);
        CustomTools.IsNull(menuScreen, nameof(menuScreen), name);
        CustomTools.IsNull(settingsScreen, nameof(settingsScreen), name);
        CustomTools.IsNull(onHomeScreen, nameof(onHomeScreen), name);
        
        #endif
    }

    private void Start()
    {
        gameOverScreen.EnableCanvasGroup(false);
        finishScreen.EnableCanvasGroup(false);
        playtimeScreen.EnableCanvasGroup(false);
        settingsScreen.EnableCanvasGroup(false);
        onHomeScreen.EnableCanvasGroup(false);
        menuScreen.EnableCanvasGroup(true);

    }

    private void OnEnable()
    {
        GameManager.OnPlayerDeath += PlayerDeathHandler;
        GameManager.OnGameStart += GameStartHandler;
        Finish.OnPlayerFinish += PlayerFinishHandler;
        UIButtonsEvents.OnHomePopUp += OnHomePopUpHandler;
        UIButtonsEvents.OnSettings += OnSettingsHandler;
    }

    private void OnDisable()
    {
        GameManager.OnPlayerDeath -= PlayerDeathHandler;
        GameManager.OnGameStart -= GameStartHandler;
        Finish.OnPlayerFinish -= PlayerFinishHandler;
        UIButtonsEvents.OnHomePopUp -= OnHomePopUpHandler;
        UIButtonsEvents.OnSettings -= OnSettingsHandler;
    }
    
    
    private void OnSettingsHandler(bool value)
    {
        settingsScreen.EnableCanvasGroup(value);
    }

    private void OnHomePopUpHandler(bool value)
    {
        onHomeScreen.EnableCanvasGroup(value);
    }

    private void GameStartHandler()
    {
        menuScreen.EnableCanvasGroup(false);
        playtimeScreen.EnableCanvasGroup(true);
    }

    private void PlayerDeathHandler()
    {
        gameOverScreen.EnableCanvasGroup(true);
    }

    private void PlayerFinishHandler()
    {
        playtimeScreen.EnableCanvasGroup(false);
        finishScreen.EnableCanvasGroup(true);
    }
}
