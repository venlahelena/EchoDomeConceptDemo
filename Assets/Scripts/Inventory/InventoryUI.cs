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
        // Simple pooling: reuse existing children if available, otherwise instantiate
        List<string> items = InventoryManager.Instance.GetItems();
        if (items == null) return;

        // Ensure the prefab is set
        if (slotPrefab == null)
        {
            Debug.LogError("slotPrefab is NULL!");
            return;
        }

        // Reuse existing children up to count, instantiate extra if needed
        int index = 0;
        for (; index < items.Count; index++)
        {
            var slotComp = UIUtils.GetOrCreateChildComponent<InventorySlot>(slotParent, slotPrefab, index);
            if (slotComp == null) continue;
            if (slotComp == null)
            {
                Debug.LogError("InventorySlot component missing on slot instance!");
                continue;
            }

            if (slotComp.itemNameText == null)
            {
                Debug.LogError("itemNameText is NULL in InventorySlot script!");
                continue;
            }

            slotComp.SetItemName(items[index]);
        }

        // Deactivate any leftover UI children
        for (; index < slotParent.childCount; index++)
        {
            slotParent.GetChild(index).gameObject.SetActive(false);
        }
    }
}