using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopContentController : MonoBehaviour
{
    [SerializeField] 
    private ShopItemsList shopItemsList;

    [SerializeField]
    private UIShopItemButton shopItemButtonOpened;

    [SerializeField] 
    private UIShopItemButton shopItemButtonClosed;

    private List<UIShopItemButton> assignedItemButtons;


    private void Awake()
    {
        assignedItemButtons = new List<UIShopItemButton>();
    }

    private void Start()
    {
        foreach (var shopItem in shopItemsList.ItemsList)
        {
            InstantiateItemButton(shopItem);
        }
    }

    private void InstantiateItemButton(ShopItem shopItem, int siblingIndex = -1)
    {
        var itemButton = shopItem.IsOpened ? 
            Instantiate(shopItemButtonOpened, transform) : Instantiate(shopItemButtonClosed, transform);
        if(siblingIndex > -1)
            itemButton.transform.SetSiblingIndex(siblingIndex);
        itemButton.Assign(this, shopItem.ItemId);
        assignedItemButtons.Add(itemButton);
    }

    public void OnUIItemButtonClick(int itemId)
    {
        var itemInListIndex = GetItemInListIndex(itemId);

        if (itemInListIndex == -1)
        {
            Debug.LogError("There is no ShopItem with id:" + itemId);
            return;
        }

        if (shopItemsList.ItemsList[itemInListIndex].IsOpened)
        {
            ShopController.S.AssignNewSkin(shopItemsList.ItemsList[itemInListIndex].Skin);
        }
        else
        {
            PurchaseItem(itemId, itemInListIndex);
        }
    }


    private void PurchaseItem(int itemId, int itemIndex)
    {
        var isItemPurchased = ShopController.S.PerformPurchase(shopItemsList, itemIndex);
        if(!isItemPurchased)
            return;

        var itemButton = GetItemButtonById(itemId);
        var siblingIndex = itemButton.transform.GetSiblingIndex();
        Destroy(itemButton.gameObject);
        InstantiateItemButton(shopItemsList.ItemsList[itemIndex], siblingIndex);
    }
    

    private UIShopItemButton GetItemButtonById(int itemId)
    {
        foreach (var itemButton in assignedItemButtons)
        {
            if(itemButton.ItemId == itemId)
            {
                return itemButton;
            }
        }
        return null;
    }
    

    private int GetItemInListIndex(int itemId)
    {
        for (var i = 0; i < shopItemsList.ItemsList.Count; i++)
        {
            if (shopItemsList.ItemsList[i].ItemId == itemId)
            {
                return i;
            }
        }
        return -1;
    }
}
