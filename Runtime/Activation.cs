using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using JiRath.InteractSystem;


namespace JiRath.InventorySystem
{
    /// <summary>
    /// Class used to activate an interactable once a set of IItemUsables are solved
    /// </summary>
    [RequireComponent(typeof(Interactable))]
    public class Activation : MonoBehaviour
    {
        [Header("Activation Config")]
        [Tooltip("A list of interactables (IItemUsable) needed to be in the 'correct' state"), SerializeReference]
        public List<Interactable> interactablesNeeded = new List<Interactable>();
        private Interactable interactableToActivate;
        protected bool hasActivated;

        void Start()
        {
            interactableToActivate = GetComponent<Interactable>();

            //Listen for updates on interactables that need to be activated
            foreach (IItemUsable interactableNeeded in interactablesNeeded)
            {
                interactableNeeded.UseItem += Activation_UseItem;
            }
        }

        void OnDestroy()
        {
            foreach (IItemUsable interactableNeeded in interactablesNeeded)
            {
                interactableNeeded.UseItem -= Activation_UseItem;
            }
        }

        //Called when there is an update to one of the listed interactables
        public void Activation_UseItem()
        {
            //Check each interactable in our list to see if they are all 'correct'
            foreach (IItemUsable interactableNeeded in interactablesNeeded)
            {
                if (!interactableNeeded.IsCorrect())
                {
                    hasActivated = false;
                    break;
                }
                else
                {
                    hasActivated = true;
                }
            }

            //Send an update to the interactable that we want to enable/disable
            interactableToActivate.Activate(hasActivated);
        }
    }
}