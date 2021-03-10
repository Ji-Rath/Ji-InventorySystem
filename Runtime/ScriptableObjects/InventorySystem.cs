using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory")]
public class Inventory : ScriptableObject
{
    public List<InventoryItem> itemList = new List<InventoryItem>();
    public int maxSlots = 3;
}

[Serializable]
public struct InventoryItem
{
    public ItemBase item;
    public int itemAmount;

    public InventoryItem(ItemBase item, int itemAmount)
    {
        this.item = item;
        this.itemAmount = itemAmount;
    }
}
