using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static event Action OnLevelLoadStarted;
    public static event Action OnLevelReloadStarted;
    
    [SerializeField] private LevelSequence levelSequence;
    [SerializeField] private PlayerData playerData;
    
    [SerializeField, Range(0f, 5f)] private float restartDelay = 0.5f;
    [SerializeField, Range(0f, 1f)] private float levelLoadDelay = 0.5f;

    public void RestartLevel()
    {
        StartCoroutine(Restart());
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadNext());
    }

    public void LoadShopLevel()
    {
        StartCoroutine(LoadShop());
    }
    
    private IEnumerator LoadNext()
    {
        OnLevelLoadStarted?.Invoke();

        playerData.currentLevel++;
        if (playerData.currentLevel >= levelSequence.LevelsList.Count)
            playerData.currentLevel = 0;
        
        SaveSystem.SavePlayerSettings(playerData);

        if(levelLoadDelay > 0)
            yield return new WaitForSecondsRealtime(levelLoadDelay);
        
        SceneManager.LoadSceneAsync(levelSequence.LevelsList[playerData.currentLevel]);
    }

    private IEnumerator LoadShop()
    {
        OnLevelLoadStarted?.Invoke();
        SaveSystem.SavePlayerSettings(playerData);
        if(levelLoadDelay > 0)
            yield return new WaitForSecondsRealtime(levelLoadDelay);
        SceneManager.LoadSceneAsync(levelSequence.ShopLevel);
    }

    private IEnumerator Restart()
    {    
        OnLevelReloadStarted?.Invoke();
        var index = SceneManager.GetActiveScene().buildIndex;
        if(restartDelay > 0f)
            yield return new WaitForSecondsRealtime(restartDelay);
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(index);
    }
}
