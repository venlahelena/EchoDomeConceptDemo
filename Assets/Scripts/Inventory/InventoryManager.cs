using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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