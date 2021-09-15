using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScreensController : MonoBehaviour
{
    [SerializeField] private Canvas gameOverScreen = default;
    [SerializeField] private Canvas finishScreen = default;
    [SerializeField] private Canvas playtimeScreen = default;
    [SerializeField] private Canvas menuScreen = default;
    [SerializeField] private Canvas settingsScreen = default;
    [SerializeField] private Canvas onHomeScreen = default;
    [SerializeField] private Canvas BlackScreen = default;

    private void Awake()
    {
        #if UNITY_EDITOR
                
        CustomTools.IsNull(gameOverScreen, nameof(gameOverScreen), name);
        CustomTools.IsNull(finishScreen, nameof(finishScreen), name);
        CustomTools.IsNull(playtimeScreen, nameof(playtimeScreen), name);
        CustomTools.IsNull(menuScreen, nameof(menuScreen), name);
        CustomTools.IsNull(settingsScreen, nameof(settingsScreen), name);
        CustomTools.IsNull(onHomeScreen, nameof(onHomeScreen), name);
        CustomTools.IsNull(BlackScreen, nameof(BlackScreen), name);
        
        #endif
        
       
    }

    private void Start()
    {
        gameOverScreen.enabled = false;
        finishScreen.enabled = false;
        playtimeScreen.enabled = false;
        settingsScreen.enabled = false;
        onHomeScreen.enabled = false;
        menuScreen.enabled = true;
        BlackScreen.enabled = true;
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
        settingsScreen.enabled = value;
    }

    private void OnHomePopUpHandler(bool value)
    {
        onHomeScreen.enabled = value;
    }

    private void GameStartHandler()
    {
        menuScreen.enabled = false;
        playtimeScreen.enabled = true;
    }

    private void PlayerDeathHandler()
    {
        gameOverScreen.enabled = true;
    }

    private void PlayerFinishHandler()
    {
        playtimeScreen.enabled = false;
        finishScreen.enabled = true;
    }
}
