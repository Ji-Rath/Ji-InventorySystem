
using UnityEngine;
using JiRath.InventorySystem;
using JiRath.InteractSystem;
using System;

namespace JiRath.InventorySystem.EquipSystem
{
    public class EquipManager : MonoBehaviour
    {
        public delegate void OnPlayerEquipAction(int chance);
        public static OnPlayerEquipAction OnPlayerDropItem;

        public event Action<Pickupable> OnUseItem;

        [HideInInspector]
        public Pickupable currentEquippedItem = null;
        public GameObject itemDisplay;
        public AudioClip dropSound;
        public float scalePreview = 1f;
        private AudioSource audioSource;
        private InteractManager interactSystem;

        public float throwForce = 10f;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            interactSystem = GetComponent<InteractManager>();
            GetComponent<InventoryManager>().UpdateInventoryEvent += CheckEquippedItem;
        }

        void Update()
        {
            if (currentEquippedItem)
            {
                Pickupable pickupable = currentEquippedItem.GetComponent<Pickupable>();
                //Using item that the player has equipped
                if (Input.GetButtonDown("Fire1"))
                {
                    pickupable.OnUse(gameObject, interactSystem.GetItemInView());
                    OnUseItem?.Invoke(pickupable);
                }
                if (Input.GetButtonDown("DropItem"))
                {
                    DropItem(pickupable.item);
                }
            }
        }

        private void UnequipItem()
        {
            // Destroy currently equipped item
            if (currentEquippedItem != null)
            {
                Destroy(currentEquippedItem.gameObject);
            }
        }

        public void EquipItem(GameObject item)
        {
            bool isSameItem = false;
            if (currentEquippedItem != null)
                isSameItem = currentEquippedItem.item == item.GetComponent<Pickupable>().item;
            UnequipItem();


            // Only equip the item if it is not already equipped 
            if (!isSameItem)
            {
                //Create new item
                GameObject InstanceItem = Instantiate(item, itemDisplay.transform);
                currentEquippedItem = InstanceItem.GetComponent<Pickupable>();
                currentEquippedItem.transform.localScale = new Vector3(scalePreview, scalePreview, scalePreview);
                currentEquippedItem.GetComponent<Collider>().enabled = false;
                currentEquippedItem.GetComponent<Rigidbody>().isKinematic = true;

                //Make sure it follows the player properly
                ItemSway itemSway = GetComponent<ItemSway>();
                if (itemSway)
                {
                    itemSway.TargetItem = currentEquippedItem.gameObject;
                    itemSway.targetLocation = itemDisplay.transform;
                }
            }
        }

        //Drop the selected item
        public void DropItem(ItemBase item)
        {
            if (!item) { Debug.Log("No Item found to drop!"); return; }

            GetComponent<InventoryManager>().RemoveFromInventory(item, 1);

            if (currentEquippedItem != null)
                Destroy(currentEquippedItem.gameObject);

            if (audioSource)
            {
                audioSource.clip = dropSound;
                audioSource.Play();
            }

            GameObject droppedItem = Instantiate(item.itemModel, gameObject.transform.position, Quaternion.identity);
            Pickupable pickup = droppedItem.GetComponent<Pickupable>();

            //Add velocity and angular velocity to thrown object
            Rigidbody itemBody = droppedItem.GetComponent<Rigidbody>();
            itemBody.velocity = Camera.main.transform.forward * throwForce;
            itemBody.angularVelocity = new Vector3(throwForce, throwForce, throwForce);

            //Cause enemy to appear
            if (pickup)
            {
                OnPlayerDropItem?.Invoke(pickup.enemySpawnChance);
            }
        }

        public void DropItem()
        {
            DropItem(currentEquippedItem.item);
        }

        private void CheckEquippedItem()
        {
            var inventory = GetComponent<InventoryManager>();
            if (!inventory.ItemExists(currentEquippedItem.item))
            {
                UnequipItem();
            }
        }

        public Pickupable GetEquippedItem()
        {
            return currentEquippedItem;
        }
    }
}