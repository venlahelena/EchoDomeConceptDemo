using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionPickup : MonoBehaviour, IInteractable
{
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    public string itemName = "Item";
    [TextArea] public string pickupMessage = "You found something!";
    public bool isPickedUp = false;

    public void Interact()
    {
        if (isPickedUp) return;

        isPickedUp = true;

        if (messagePanel && messageText)
        {
            messagePanel.SetActive(true);
            messageText.text = pickupMessage;
        }

        InventoryManager.Instance?.AddItem(itemName);
        Debug.Log("Picked up: " + itemName);
    }
}
