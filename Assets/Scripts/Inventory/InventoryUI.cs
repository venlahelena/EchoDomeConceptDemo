using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject slotPrefab;
    public Transform slotParent;
    public GameObject inventoryPanel;

    void OnEnable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged += RefreshUI;

        // Optional: Refresh UI on enable if inventory exists
        if (InventoryManager.Instance != null)
            RefreshUI();
    }

    void OnDisable()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            bool isActive = inventoryPanel.activeSelf;
            inventoryPanel.SetActive(!isActive);

            if (inventoryPanel.activeSelf)
            {
                RefreshUI();
            }
        }
    }

    public void RefreshUI()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogWarning("InventoryManager instance is null!");
            return;
        }

        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        List<string> items = InventoryManager.Instance.GetItems();
        if (items == null) return;

        foreach (string item in items)
        {
            if (slotPrefab == null)
            {
                Debug.LogError("slotPrefab is NULL!");
                return;
            }

            GameObject slot = Instantiate(slotPrefab, slotParent);
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();

            if (inventorySlot == null)
            {
                Debug.LogError("InventorySlot component missing on slot prefab instance!");
                continue;
            }

            if (inventorySlot.itemNameText == null)
            {
                Debug.LogError("itemNameText is NULL in InventorySlot script!");
                continue;
            }

            inventorySlot.SetItemName(item);
        }
    }
}