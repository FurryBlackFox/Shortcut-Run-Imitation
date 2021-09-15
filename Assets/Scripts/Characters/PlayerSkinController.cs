using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerSkinController : CharacterSkinController
{
    private Player player;
    
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    private void Start()
    {
        SetSkin(player.PlayerData.currentSkin);
    }
}
