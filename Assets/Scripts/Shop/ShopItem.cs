using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ShopItem
{
    [SerializeField] private CharacterSkin skin;
    [SerializeField] private Image preview;
    [SerializeField] private string name;
    [SerializeField] private int itemId;
    [SerializeField] private bool isOpened;

    public CharacterSkin Skin => skin;

    public Image Preview => preview;

    public string Name => name;
    
    public int ItemId => itemId;
    
    public bool IsOpened
    {
        get => isOpened;
        set => isOpened = value;
    }
}
