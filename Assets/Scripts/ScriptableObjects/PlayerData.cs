using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Player Data", menuName = "Scriptable Objects/Player Data", order = 2)]
public class PlayerData : ScriptableObject
{
    [SerializeField] internal Material plankMaterialOpaque = default;
    [SerializeField, Min(0)] internal int coinsCount = 0;
    [SerializeField] internal string playerName = "Player";
    [SerializeField] internal int currentLevel = 0;
    
    [SerializeField] internal CharacterSkin currentSkin;
    // TODO: finish after implementing shop
    
    public Material PlankMaterialOpaque => plankMaterialOpaque;


    public string PlayerName
    {
        get => playerName;
        set => playerName = value;
    }

    public int CoinsCount
    {
        get => coinsCount;
        set => coinsCount = value;
    }

    public int CurrentLevel
    {
        get => currentLevel;
        set => currentLevel = value;
    }

    public CharacterSkin CurrentSkin
    {
        get => currentSkin;
        set => currentSkin = value;
    }
}
