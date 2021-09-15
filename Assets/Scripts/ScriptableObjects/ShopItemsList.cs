using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Shop Items List", menuName = "Scriptable Objects/Shop Items List", order = 13)]
public class ShopItemsList : ScriptableObject
{
    [SerializeField] private List<ShopItem> itemsList;

    [SerializeField] private int cost;
    
    public List<ShopItem> ItemsList => itemsList;

    public int Cost => cost;
}
