using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class InventorySlot : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;

    public void SetItemName(string name)
    {
        if (itemNameText != null)
            itemNameText.text = name;
        else
            Debug.LogWarning("ItemNameText reference missing in InventorySlot!");
    }
}
