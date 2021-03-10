using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemReadable", menuName = "Item/Readable Item")]
public class ItemReadable : ItemBase
{
    [Tooltip("The body of text to display"), TextArea]
    public string content;
}
