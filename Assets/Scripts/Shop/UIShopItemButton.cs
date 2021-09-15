using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIShopItemButton : MonoBehaviour, IPointerClickHandler
{
    private ShopContentController shopContentController;
    
    public int ItemId { get; private set; }
    
    public void Assign(ShopContentController shopController, int itemId)
    {
        this.shopContentController = shopController;
        this.ItemId = itemId;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        shopContentController.OnUIItemButtonClick(ItemId);
    }
}
