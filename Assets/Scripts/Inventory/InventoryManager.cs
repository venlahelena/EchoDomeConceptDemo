using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple inventory manager storing item names. Provides Add/Remove/Has APIs and change events for UI.
/// </summary>
/* Editor notes:
 - This manager persists only in-memory. For testing, call AddItem("Filter") at runtime to give the player a filter.
 - Subscribe to `OnInventoryChanged` to update UI lists.
 - Consider replacing the string-based items with a ScriptableObject item system later.
*/
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    private List<string> items = new List<string>();

    // Event to notify listeners when inventory changes
    public event System.Action OnInventoryChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    GameObjectUtils.PreserveRoot(this);
    }

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log("Added to inventory: " + itemName);
        OnInventoryChanged?.Invoke();
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public void RemoveItem(string itemName)
    {
        if (items.Contains(itemName))
        {
            items.Remove(itemName);
            Debug.Log("Removed from inventory: " + itemName);
            OnInventoryChanged?.Invoke();
        }
    }

    public List<string> GetItems()
    {
        return items;
    }
}