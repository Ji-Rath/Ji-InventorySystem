using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemReadableImage", menuName = "Item/Image Item")]
public class ItemReadableImage : ItemBase
{
    [Tooltip("The image to display")]
    public Sprite image;
}
