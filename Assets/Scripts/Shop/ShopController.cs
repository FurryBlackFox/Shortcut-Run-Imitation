using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{ 
    public static ShopController S;

    [SerializeField] 
    private PlayerData playerData;


    private void Awake()
    {
        if (S == null)
            S = this;
    }

    private void Start()
    {
        ShopSkinsRepresentator.S.RepresentNewSkin(playerData.currentSkin);
    }

    public bool PerformPurchase(ShopItemsList shopItemsList, int itemIndex)
    {
        //TODO:checks
        shopItemsList.ItemsList[itemIndex].IsOpened = true;
        var newSkin = shopItemsList.ItemsList[itemIndex].Skin;
        AssignNewSkin(newSkin);
        
        return true;
    }

    public void AssignNewSkin(CharacterSkin newSkin)
    {
        playerData.currentSkin = newSkin;
        ShopSkinsRepresentator.S.RepresentNewSkin(newSkin);
    }
}
