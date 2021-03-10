
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using JiRath.InteractSystem.UI;
using JiRath.InteractSystem;
using JiRath.InventorySystem.EquipSystem;

namespace JiRath.InventorySystem.UI
{
    [RequireComponent(typeof(Animator))]
    public class ReadUI : UIBase
    {
        public static ReadUI instance;

        public TMP_Text textRead;
        public Image imageRead;
        private Animator interactAnimator;

        private bool isVisible = false;

        public override bool IsEnabled()
        {
            //Make sure transition animation isnt playing while determining visibility

            if (interactAnimator)
                return isVisible || interactAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Transition");

            return isVisible;
        }

        void Start()
        {
            instance = this;
            interactAnimator = GetComponent<Animator>();
        }

        public void UI_UseItem(Pickupable pickupable)
        {
            if (CanEnable() && !isVisible)
            {
                //If item is not valid, return
                if (!pickupable) { Debug.LogError("Unable to access pickupable!"); return; }

                ItemReadable readable = pickupable.item as ItemReadable;
                ItemReadableImage readableImage = pickupable.item as ItemReadableImage;
                if (readable)
                {
                    //Display text and make UI visible
                    textRead.SetText(readable.content);
                    textRead.enabled = true;
                    imageRead.enabled = false;
                }
                else if (readableImage)
                {
                    //Display image and make UI visible
                    imageRead.sprite = readableImage.image;
                    textRead.enabled = false;
                    imageRead.enabled = true;
                }
                else
                {
                    //If item is not readable, return
                    return;
                }

                DisablePlayer(true);
                isVisible = true;
                interactAnimator.SetBool("isVisible", isVisible);
            }
        }

        public void UI_CancelAction()
        {
            if (isVisible)
            {
                isVisible = false;
                interactAnimator.SetBool("isVisible", isVisible);
                DisablePlayer(false);
            }
        }

        void OnDestroy()
        {
            if (owningPlayer)
            {
                owningPlayer.GetComponent<InteractManager>().OnCancelAction -= UI_CancelAction;
                owningPlayer.GetComponent<EquipManager>().OnUseItem -= UI_UseItem;
            }
        }

        public override void Bind(GameObject owner)
        {
            base.Bind(owner);
            if (owner)
            {
                owner.GetComponent<InteractManager>().OnCancelAction += UI_CancelAction;
                owner.GetComponent<EquipManager>().OnUseItem += UI_UseItem;
            }
        }
    }
}