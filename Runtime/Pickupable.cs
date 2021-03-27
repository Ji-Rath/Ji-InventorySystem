using System;
using UnityEditor;
using UnityEngine;
using JiRath.InteractSystem;


namespace JiRath.InventorySystem
{
    /// <summary>
    /// Class that handles objects that can be picked up by the player and optionally used
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Pickupable : Interactable
    {
        [Tooltip("The item that will be given to the player when interacted")]
        public ItemBase item;
        [Range(0, 100)]
        public int enemySpawnChance;

        [Header("Sound Config"), Space]
        public AudioClip pickupSound;
        public AudioClip useSound;

		private AudioSource audioSource;
        protected InventoryManager playerInventory;
        protected InteractManager playerInteract;

        [Tooltip("Message displayed when item is successfully used"), TextArea]
        public string onUseMessage;

        public event Action<GameObject> PickupEvent;

        public override void OnInteract(GameObject Interactor)
        {
            base.OnInteract(Interactor);

            GameObject playerRef = Interactor;
            playerInventory = playerRef.GetComponent<InventoryManager>();
            playerInteract = playerRef.GetComponent<InteractManager>();
            audioSource = playerRef.GetComponent<AudioSource>();

            //If the item is pickupable, add it to inventory
            if (playerInventory.AddToInventory(item))
            {
                if (audioSource)
                {
                    audioSource.clip = pickupSound;
                    audioSource.Play();
                }

                PickupEvent?.Invoke(Interactor);
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Called when the item will be used
        /// </summary>
        public bool OnUse(GameObject user, Interactable itemInView)
        {
            playerInventory = user.GetComponent<InventoryManager>();
            playerInteract = user.GetComponent<InteractManager>();

            //If the item could be used, play sound
            if (Use(user, itemInView))
            {
                //Play use sound
                audioSource?.PlayOneShot(useSound);

                //Show used message
                UpdateUseText();
                //DialogueBox.Get().TriggerText(onUseMessage);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Update useMessage with appropriate text related to the item, can probably be put in a static class later on
        /// </summary>
        void UpdateUseText()
        {
            Interactable itemInView = playerInteract.GetItemInView();
            onUseMessage = onUseMessage.Replace("{item}", item.name);
            if (itemInView)
                onUseMessage = onUseMessage.Replace("{itemInView}", itemInView.name);
        }

        /// <summary>
        /// Functionality of using an item on a IItemUsable component
        /// </summary>
        /// <returns>Whether the item could be used.</returns>
        public virtual bool Use(GameObject user, Interactable itemInView)
        {
            if (itemInView)
            {
                IItemUsable itemUsable = itemInView.GetComponent<IItemUsable>();
                if (itemUsable != null)
                    return itemUsable.OnItemUse(user, item);
            }
            return false;
        }

        public override bool CanInteract(GameObject Interactor)
        {
            return true;
        }

        public override string GetName()
        {
            string name = base.GetName();
            if (item)
                name = item.name;

            return name;
        }

        private void Reset()
        {
            nameVisible = true;
        }
    }
}
