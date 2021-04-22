using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem //TODO: implement after playerSettings script
{

    private static string playerNameKey = "Player Name";
    private static string coinsCountKey = "Coins Count";
    private static string currentLevelKey = "Current Level";

    public static void SavePlayerSettings(PlayerData playerData)
    {
        PlayerPrefs.SetString(playerNameKey, playerData.PlayerName);
        PlayerPrefs.SetInt(coinsCountKey, playerData.CoinsCount);
        PlayerPrefs.SetInt(currentLevelKey, playerData.CurrentLevel);
        PlayerPrefs.Save();
    }

    public static void LoadPlayerSettings(ref PlayerData playerData)
    { 
        playerData.PlayerName = PlayerPrefs.GetString(playerNameKey, "Player");
        playerData.coinsCount = PlayerPrefs.GetInt(coinsCountKey, 0);
        playerData.currentLevel = PlayerPrefs.GetInt(currentLevelKey, 0);
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
