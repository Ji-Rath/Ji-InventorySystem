using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JiRath.InventorySystem.EquipSystem
{
    [RequireComponent(typeof(EquipManager))]
    public class ItemSway : MonoBehaviour
    {
        [HideInInspector]
        public Transform targetLocation;
        [HideInInspector]
        public GameObject TargetItem;

        public float smoothTime = 3f;
        public float maxSway = 0.2f;
        public float swaySensitivity = 0.1f;

        void LateUpdate()
        {
            //Item Sway
            if (TargetItem != null && targetLocation != null)
            {
                //Get values of mouse input
                float moveX = Input.GetAxis("Mouse X") * swaySensitivity;
                float moveY = Input.GetAxis("Mouse Y") * swaySensitivity;

                //Clamp values
                moveX = Mathf.Clamp(moveX, -maxSway, maxSway);
                moveY = Mathf.Clamp(moveY, -maxSway, maxSway);

                //Update position of object
                Vector3 desiredLocation = new Vector3(moveX, moveY, 0f);
                TargetItem.transform.localPosition = Vector3.Lerp(TargetItem.transform.localPosition, desiredLocation, smoothTime * Time.deltaTime);
            }
        }
    }
}