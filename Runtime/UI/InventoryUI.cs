using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JiRath.InteractSystem.UI;
using JiRath.InventorySystem.EquipSystem;

namespace JiRath.InventorySystem.UI
{
    public class InventoryUI : UIBase
    {
        InventoryManager playerInventory;

        // Get UI references
        public Button nextButton;
        public Button prevButton;
        public Button equipButton;
        public TMP_Text textName;
        public TMP_Text textDescription;
        public TMP_Text textAmount;
        
        [Space]
        public Transform itemPrefabPosition;
        public float scalePreview = 1;
        private GameObject itemPrefab;

        [Space]
        public bool loopInventory = false;

        private int viewedSlot = 0;
        private bool inventoryVisible = false;

        public override bool IsEnabled()
        {
            return inventoryVisible;
        }

        void Start()
        {
            canvasReference.enabled = false;
        }

        private void CreateItemPrefab(InventoryItem currentItem)
        {
            //Create the selected inventory prefab, scale it, and delete the old one
            Destroy(itemPrefab);
            GameObject newInventoryPrefab = Instantiate(currentItem.item.itemModel, itemPrefabPosition);
            newInventoryPrefab.transform.localScale = new Vector3(scalePreview, scalePreview, scalePreview);

            //Add InventoryPrefab component for spin effect
            newInventoryPrefab.AddComponent<InventoryPrefab>();
            newInventoryPrefab.GetComponent<Rigidbody>().isKinematic = true;

            itemPrefab = newInventoryPrefab;
        }

        void UpdateInventoryUI()
        {
            if (CanEnable() && inventoryVisible)
            {
                //Only try displaying item if there are any contents
                int itemCount = playerInventory.inventory.itemList.Count;
                if (itemCount > 0)
                {
                    InventoryItem currentItem = playerInventory.inventory.itemList[viewedSlot];
                    if (currentItem.item != null)
                    {
                        //Update text
                        textName.SetText(currentItem.item.name);
                        textDescription.SetText(currentItem.item.description);

                        //Do not display stack count if the max stack is 1
                        int itemStack = currentItem.item.maxStack;
                        if (itemStack > 1)
                        {
                            textAmount.text = "Amount: " + currentItem.itemAmount + " / " + itemStack;
                        }
                        else
                        {
                            textAmount.text = "";
                        }

                        CreateItemPrefab(currentItem);
                    }
                }
            }
        }

        public void SetVisibility(bool isVisible)
        {
            CheckValidSlot();
            inventoryVisible = isVisible;
            canvasReference.enabled = isVisible;
            EmptyInventory();
            UpdateInventoryUI();
        }

        public void EquipCurrentItem()
        {
            if (playerInventory && playerInventory.inventory.itemList.Count > 0)
            {
                EquipManager equipSystem = owningPlayer.GetComponent<EquipManager>();
                GameObject itemPrefab = playerInventory.inventory.itemList[viewedSlot].item.itemModel;
                equipSystem.EquipItem(itemPrefab);
            }
        }

        //Clear all item text from the inventory
        void EmptyInventory()
        {
            textName.text = "";
            textDescription.text = "";
            textAmount.text = "";

            if (itemPrefab != null)
                Destroy(itemPrefab);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (playerInventory)
            {
                playerInventory.UpdateInventoryEvent -= UpdateInventoryUI;
                playerInventory.OnToggleInventory -= SetVisibility;
                playerInventory.OnToggleInventory -= DisablePlayer;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Bind(owningPlayer);
        }

        public void GetNextItem()
        {
            IncrementSlot(1);
            playerInventory.GetItem(viewedSlot);
            UpdateInventoryUI();
        }
        public void GetPreviousItem()
        {
            IncrementSlot(-1);
            playerInventory.GetItem(viewedSlot);
            UpdateInventoryUI();
        }

        private void CheckValidSlot()
        {
            int maxSlots = playerInventory.inventory.itemList.Count;
            if (viewedSlot >= maxSlots)
            {
                if (loopInventory)
                {
                    viewedSlot -= maxSlots;
                }
                else
                {
                    viewedSlot = maxSlots != 0 ? maxSlots-1 : 0;
                }
            }
            else if (viewedSlot < 0)
            {
                if (loopInventory)
                {
                    viewedSlot = maxSlots - Mathf.Abs(viewedSlot);
                }
                else
                {
                    viewedSlot = 0;
                }
            }

            if (equipButton)
            {
                equipButton.interactable = playerInventory.inventory.itemList.Count != 0 ? true : false;
            }

            if (!loopInventory && nextButton && prevButton)
            {
                prevButton.interactable = true;
                nextButton.interactable = true;
                prevButton.interactable = viewedSlot == 0 ? false : true;
                nextButton.interactable = viewedSlot >= maxSlots-1 ? false : true;
            }
        }

        public int IncrementSlot(int amountChange)
        {
            viewedSlot += amountChange;
            CheckValidSlot();


            return Mathf.Abs(viewedSlot);
        }

        public override void Bind(GameObject owner)
        {
            base.Bind(owner);

            if (owner)
            {
                playerInventory = owner.GetComponent<InventoryManager>();
                playerInventory.UpdateInventoryEvent += UpdateInventoryUI;
                playerInventory.OnToggleInventory += SetVisibility;
                playerInventory.OnToggleInventory += DisablePlayer;
                //MonsterAI.OnMonsterKillPlayer += DisableInventoryUI;
            }
        }
    }
}