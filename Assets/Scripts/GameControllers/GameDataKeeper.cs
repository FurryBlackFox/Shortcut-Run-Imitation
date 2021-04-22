﻿using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


[ExecuteAlways]
public class GameDataKeeper : MonoBehaviour
{
    public static GameDataKeeper S;

    
    public List<Character> characters;

    [SerializeField] private GameSettings gameSettings = default;
    public GameSettings GameSettings => gameSettings;

    [SerializeField] private LevelSequence levelSequence = default;
    public LevelSequence LevelSequence => levelSequence;

    [SerializeField] private BotSettings botSettings = default;
    public BotSettings BotSettings => botSettings;

    [SerializeField] private WaypointSettings waypointSettings;
    public WaypointSettings WaypointSettings => waypointSettings;

    [SerializeField] private Waypoint enterWaypoint = default;
    public Waypoint EnterWaypoint => enterWaypoint;
    
    [SerializeField] private Bot botPrefab = default;
    public Bot BotPrefab => botPrefab;
    
    

    public Player Player { get; private set; }
    public CinemachineBrain CinemachineBrain { get; private set; }
    public PlayerData PlayerData { get; private set; }
    
    public PlankObjectPool PlankObjectPool { get; private set; }


    private void OnEnable() //Cause executes in editor too 
    {
#if UNITY_EDITOR
        
        CustomTools.IsNull(gameSettings, nameof(gameSettings), name);
        CustomTools.IsNull(botPrefab, nameof(botPrefab), name);
        CustomTools.IsNull(enterWaypoint, nameof(enterWaypoint), name);
        CustomTools.IsNull(botSettings, nameof(botSettings), name);

#endif
        if (!S)
            S = this;
        else
            Debug.LogError("There are multiple GameDataKeepers in the scene !");
        
        characters = new List<Character>();
        Player = FindObjectOfType<Player>();
        characters.Add(Player);
        characters.AddRange(FindObjectsOfType<Bot>());
        CinemachineBrain = FindObjectOfType<CinemachineBrain>();
        PlankObjectPool = FindObjectOfType<PlankObjectPool>();
        PlayerData = Player.PlayerData;
    }

    private void OnDisable()
    {
        S = null;
    }
}
