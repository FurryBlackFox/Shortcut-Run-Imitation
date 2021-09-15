using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static event Action OnPlayerDeath;
    public static event Action OnGameStart;
    public static event Action<int> OnPlayerCoinsChanged;

    public static bool isGameEnded;
    
    [SerializeField, Min(-1)] private int targetFramerate = 0;
    [SerializeField] private bool enableMobileVSync = true;


    
    private Player player;
    private PlayerData playerData;
    private LevelLoader levelLoader;
    private void Awake()
    {
        isGameEnded = false;
        Application.targetFrameRate = targetFramerate;
        
    }

    private void OnEnable()
    {
        player = GameDataKeeper.S.Player;
        levelLoader = GameDataKeeper.S.LevelLoader;
        player.OnDrowning += OnGameOver;
        
        Finish.OnEveryoneFinished += RaceOver;
        UIButtonsEvents.OnPlay += OnStartHandler;
        UIButtonsEvents.OnRestart += OnRestartHandler;
        UIButtonsEvents.OnHomePopUp += OnHomePopUpHandler;
        UIButtonsEvents.OnClaimPoints += OnClaimPointsHandler;
        UIButtonsEvents.OnSettings += OnSettingsHandler;
        UIButtonsEvents.OnHome += OnHomeHandler;
        UIButtonsEvents.OnShop += OnShopHandler;
    }

    private void OnDisable()
    {
        player.OnDrowning -= OnGameOver;
        Finish.OnEveryoneFinished -= RaceOver;
        UIButtonsEvents.OnPlay -= OnStartHandler;
        UIButtonsEvents.OnRestart -= OnRestartHandler;
        UIButtonsEvents.OnHomePopUp -= OnHomePopUpHandler;
        UIButtonsEvents.OnClaimPoints -= OnClaimPointsHandler;
        UIButtonsEvents.OnSettings -= OnSettingsHandler;
        UIButtonsEvents.OnHome -= OnHomeHandler;
        UIButtonsEvents.OnShop -= OnShopHandler;
    }
    
    private void Start()
    {
        playerData = GameDataKeeper.S.PlayerData;

        if (!enableMobileVSync && Application.isMobilePlatform)
            QualitySettings.vSyncCount = 0;

        SaveSystem.LoadPlayerSettings(ref playerData);
        OnPlayerCoinsChanged?.Invoke(playerData.CoinsCount);
        
        SetPlankSettings();
    }

    private void SetPlankSettings()
    {
        var gameSettings = GameDataKeeper.S.GameSettings;
        TriggerPlank.RespawnPlanks = gameSettings.RespawnPlanks;
        TriggerPlank.RespawnTime = gameSettings.PlankRespawnTime;
    }


    
    private void OnStartHandler()
    {
        OnGameStart?.Invoke();
    }

    private void OnClaimPointsHandler(int points)
    {
        player.AddCoins(points);
        OnPlayerCoinsChanged?.Invoke(playerData.CoinsCount);
        SaveSystem.SavePlayerSettings(playerData);
        
        levelLoader.LoadNextLevel();
    }

    private void OnSettingsHandler(bool value)
    {
        
    }

    private void OnHomeHandler()
    {
        levelLoader.RestartLevel();
    }

    private void OnHomePopUpHandler(bool value)
    {
        Time.timeScale = value ? 0f : 1f;
    }

    private void OnShopHandler()
    {
        levelLoader.LoadShopLevel();
    }
    
    private void OnRestartHandler()
    {

        levelLoader.RestartLevel();
    }

    private static void OnGameOver()
    {
        OnPlayerDeath?.Invoke();
        isGameEnded = true;
    }

    private void RaceOver() //TODO: complete
    {
        isGameEnded = true;
    }
}
