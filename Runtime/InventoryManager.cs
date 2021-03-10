using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JiRath.InventorySystem
{
    public class InventoryManager : MonoBehaviour
    {

        // Event called when there is an update to the inventory
        public event Action UpdateInventoryEvent;

        public event Action<bool> OnToggleInventory;

        // ScriptableObject that contains inventory items/info
        public Inventory inventory;

        [HideInInspector]
        private bool inventoryVisible = false;

        void Update()
        {
            //Check if inventory key was pressed
            if (Input.GetButtonDown("Inventory"))
            {
                ToggleInventory();
            }
        }

        // Toggle visibility of inventory
        private void ToggleInventory()
        {
            inventoryVisible = !inventoryVisible;
            OnToggleInventory?.Invoke(inventoryVisible);
        }

        //Called when the player wants to view the next item in their inventory
        public InventoryItem GetItem(int index)
        {
            if (index < inventory.itemList.Count && index >= 0)
                return inventory.itemList[index];

            return new InventoryItem();
        }

        public bool ItemExists(ItemBase item)
        {
            foreach (InventoryItem invItem in inventory.itemList)
            {
                if (invItem.item == item)
                {
                    return true;
                }
            }
            return false;
        }

        //Add specified item to the players inventory if possible
        public bool AddToInventory(ItemBase item, int amount = 1)
        {
            // Cycle through all inventory slots
            for (int i = 0; i < inventory.itemList.Count; i++)
            {
                InventoryItem itemSlot = inventory.itemList[i];
                ItemBase itemTest = itemSlot.item;

                // If the player has the item already in their inventory, attempt to add to stack
                if (itemTest && itemTest.Equals(item) && (itemSlot.itemAmount + amount) <= itemTest.maxStack)
                {
                    AddToStack(i, amount);
                    UpdateInventoryEvent?.Invoke();
                    return true;
                }
            }

            //If there is an empty slot available, use that instead
            if (inventory.itemList.Count < inventory.maxSlots)
            {
                inventory.itemList.Add(new InventoryItem(item, amount));
                return true;
            }

            //Unable to add item to inventory
            return false;
        }

        public bool CanAdd(ItemBase item, int amount = 1)
        {
            // Cycle through all inventory slots
            for (int i = 0; i < inventory.itemList.Count; i++)
            {
                InventoryItem itemSlot = inventory.itemList[i];
                ItemBase itemTest = itemSlot.item;

                // If the player has the item already in their inventory, attempt to add to stack
                if (itemTest && itemTest.Equals(item) && (itemSlot.itemAmount + amount) <= itemTest.maxStack)
                {
                    return true;
                }
            }

            //If there is an empty slot available, use that instead
            if (inventory.itemList.Count < inventory.maxSlots)
            {
                return true;
            }

            //Unable to add item to inventory
            return false;
        }

        //Remove specified item/amount from inventory
        public bool RemoveFromInventory(ItemBase item, int amount)
        {
            for (int i = 0; i < inventory.itemList.Count; i++)
            {
                //When the item is found, remove the set amount
                if (inventory.itemList[i].item == item)
                {
                    InventoryItem itemSlot = inventory.itemList[i];
                    itemSlot.itemAmount -= amount;
                    inventory.itemList[i] = itemSlot;

                    if (itemSlot.itemAmount <= 0)
                    {
                        inventory.itemList.RemoveAt(i);
                    }
                    UpdateInventoryEvent?.Invoke();
                    return true;
                }
            }

            Debug.Log("Unable to remove item!");
            return false;
        }

        //Add specified item to the inventory (assuming it was possible)
        public void AddToStack(int slotIndex, int amount)
        {
            InventoryItem itemSlot = inventory.itemList[slotIndex];
            itemSlot.itemAmount += amount;

            inventory.itemList[slotIndex] = itemSlot;
        }

        public void OnDestroy()
        {
            //Clean Scriptable Object
            inventory.itemList.Clear();
        }
    }
}