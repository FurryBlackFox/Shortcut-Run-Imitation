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
    [SerializeField] private bool spawnBots = false;
    [SerializeField, Range(0, 4)] private int botsCount = 3;
    [SerializeField, Range(0f, 5f)] private float restartDelay = 2f;


    private Player player;
    private PlayerData playerData;
    private void Awake()
    {

        
        isGameEnded = false;
        Application.targetFrameRate = targetFramerate;
        
    }

    private void OnEnable()
    {
        player = GameDataKeeper.S.Player;
        player.OnDrowning += GameOver;
        
        Finish.OnEveryoneFinished += RaceOver;
        UIButtonsEvents.OnPlay += OnStartHandler;
        UIButtonsEvents.OnRestart += OnRestartHandler;
        UIButtonsEvents.OnHomePopUp += OnHomePopUpHandler;
        UIButtonsEvents.OnClaimPoints += OnClaimPointsHandler;
        UIButtonsEvents.OnSettings += OnSettingsHandler;
        UIButtonsEvents.OnHome += OnHomeHandler;
    }

    private void OnDisable()
    {
        player.OnDrowning -= GameOver;
        Finish.OnEveryoneFinished -= RaceOver;
        UIButtonsEvents.OnPlay -= OnStartHandler;
        UIButtonsEvents.OnRestart -= OnRestartHandler;
        UIButtonsEvents.OnHomePopUp -= OnHomePopUpHandler;
        UIButtonsEvents.OnClaimPoints -= OnClaimPointsHandler;
        UIButtonsEvents.OnSettings -= OnSettingsHandler;
        UIButtonsEvents.OnHome -= OnHomeHandler;
    }
    
    private void Start()
    {
        playerData = GameDataKeeper.S.PlayerData;

        if (!enableMobileVSync && Application.isMobilePlatform)
            QualitySettings.vSyncCount = 0;
        
        if(spawnBots)
            SpawnBots();

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

    private void SpawnBots()
    {
        for (var i = 0; i < botsCount; i++)
        {
            var bot = Instantiate(GameDataKeeper.S.BotPrefab, Vector3.zero, Quaternion.identity);
            bot.name = $"Bot {i}";

            GameDataKeeper.S.characters.Add(bot.GetComponent<Character>());
        }
    }
    
    private void OnStartHandler()
    {
        OnGameStart?.Invoke();
    }

    private void OnClaimPointsHandler(int points, bool isBonus)
    {
        player.AddCoins(points);
        OnPlayerCoinsChanged?.Invoke(playerData.CoinsCount);
        SaveSystem.SavePlayerSettings(playerData);
        LoadNextLevel();
    }

    private void OnSettingsHandler(bool value)
    {
        
    }

    private void OnHomeHandler()
    {
        Time.timeScale = 1f;
        StartCoroutine(RestartLevel());
    }

    private void OnHomePopUpHandler(bool value)
    {
        Time.timeScale = value ? 0f : 1f;
    }
    
    private void OnRestartHandler()
    {
        StartCoroutine(RestartLevel());
    }

    private void LoadNextLevel()
    {
        var levelsList = GameDataKeeper.S.LevelSequence.LevelsList;
        
        playerData.currentLevel++;
        if (playerData.currentLevel >= levelsList.Count)
            playerData.currentLevel = 0;
        
        SaveSystem.SavePlayerSettings(playerData);
        
        SceneManager.LoadSceneAsync(levelsList[playerData.currentLevel]);
    }

    private IEnumerator RestartLevel()
    {
        var index = SceneManager.GetActiveScene().buildIndex;
        if(restartDelay > 0f)
            yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadSceneAsync(index);
    }
    
    private static void GameOver()
    {
        OnPlayerDeath?.Invoke();
        isGameEnded = true;
    }

    private void RaceOver() //TODO: complete
    {
        isGameEnded = true;
    }
}
