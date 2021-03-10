using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Base Item")]
public class ItemBase : ScriptableObject
{
    public new string name;
    [TextArea]
    public string description;

    public GameObject itemModel;

    public int maxStack = 1;
}
