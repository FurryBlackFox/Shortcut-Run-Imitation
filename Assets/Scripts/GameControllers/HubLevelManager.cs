using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubLevelManager : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private LevelSequence levelSequence;

    private void Awake()
    {
        SaveSystem.LoadPlayerSettings(ref playerData);
        SceneManager.LoadSceneAsync(levelSequence.LevelsList[playerData.currentLevel]);
    }
}
