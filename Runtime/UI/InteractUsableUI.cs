using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JiRath.InteractSystem.UI;
using JiRath.InteractSystem;
using JiRath.InventorySystem.EquipSystem;
using JiRath.InventorySystem;

public class InteractUsableUI : InteractUI
{

    protected override string GetInteractableMessage(Interactable interactable)
    {
        string returnString = base.GetInteractableMessage(interactable);

        IItemUsable usableInteractable = interactable as IItemUsable;
        bool canUseItem = usableInteractable != null && usableInteractable.CanUse();
        if (owningPlayer && canUseItem)
        {
            var equip = owningPlayer.GetComponent<EquipManager>();
            if (equip && equip.GetEquippedItem())
            {
                return "Use " + equip.GetEquippedItem().GetName() + " on " + interactable.GetName();
            }
        }

        return returnString;
    }
}
