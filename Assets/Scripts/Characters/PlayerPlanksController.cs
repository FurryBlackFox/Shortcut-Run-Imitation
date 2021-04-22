using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerPlanksController : CharacterPlanksController
{
    public event Action<Vector3> OnPlankCollected;
    public event Action<int> OnPlanksOverload;
    
    
    private Player player;
    
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
    
    protected override void Start()
    {
        base.Start();
        currentPlanksMaterial = planksMaterialOpaque = player.PlayerData.PlankMaterialOpaque;
    }
    
        
    internal override Vector3 AddPlank()
    {
        var pos = base.AddPlank();
        CheckPlankCount();
        OnPlankCollected?.Invoke(pos);
        return pos;
    }

    internal override void SpawnPlank(Transform point)
    {
        base.SpawnPlank(point);
        CheckPlankCount();
    }
    
    internal void CheckPlankCount()
    {
        var k = planks.Count / gameSettings.OversizeCount;
        k *= gameSettings.OversizeCount;
        OnPlanksOverload?.Invoke(k);
    }

}
