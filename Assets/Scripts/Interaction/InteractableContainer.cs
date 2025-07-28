using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractableContainer : MonoBehaviour, IInteractable
{
    [Header("UI References")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    [Header("Container Settings")]
    public string itemName = "Repair Part";
    public string openMessage = "You found a Repair Part!";
    public bool isOpened = false;

    public void Interact()
    {
        if (isOpened) return;

        isOpened = true;

        if (messagePanel != null && messageText != null)
        {
            messageText.text = openMessage;
            messagePanel.SetActive(true);
        }

        InventoryManager.Instance.AddItem(itemName);
        Debug.Log("Player picked up: " + itemName);
    }
}
