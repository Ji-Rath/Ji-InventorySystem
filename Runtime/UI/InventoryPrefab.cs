using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JiRath.InventorySystem
{
    public class InventoryPrefab : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            // Rotate item
            transform.Rotate(new Vector3(0f, 0.5f, 0f), Space.Self);
        }
    }
}