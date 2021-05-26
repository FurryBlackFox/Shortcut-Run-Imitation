using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
        
    [SerializeField] private Bot botPrefab = default;
    [SerializeField] private bool spawnBots = false;
    [SerializeField, Range(0, 4)] private int botsCount = 3;


    private void Awake()
    {
        if(spawnBots)
            SpawnBots();
    }
    
    private void SpawnBots()
    {
        for (var i = 0; i < botsCount; i++)
        {
            var bot = Instantiate(botPrefab, Vector3.zero, Quaternion.identity);
            bot.name = $"Bot {i}";

            GameDataKeeper.S.characters.Add(bot.GetComponent<Character>());
        }
    }
}
