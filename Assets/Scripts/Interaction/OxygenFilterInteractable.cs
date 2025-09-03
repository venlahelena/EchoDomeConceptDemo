using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactable representing an oxygen filter that can be replaced using an inventory item.
/// Requirements (configured in inspector):
/// - filterItemName: the inventory item required (string)
/// - oxygenRestoreAmount: how much oxygen to add when repaired
/// - successMessagePanel & successMessageText: optional UI to show a message
/// - optional logID to unlock via GameStateManager when repaired
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class OxygenFilterInteractable : MonoBehaviour, IInteractable
{
    [Header("Repair Settings")]
    public string filterItemName = "Filter";
    public float oxygenRestoreAmount = 10f;

    [Header("Optional UI Feedback")]
    public GameObject successMessagePanel;
    public TMPro.TextMeshProUGUI successMessageText;
    [TextArea]
    public string successMessage = "You replaced the oxygen filter. Oxygen levels have improved.";

    [Header("Optional Log Unlock")]
    public string unlockLogID;

    bool repaired = false;

    public void Interact()
    {
        if (repaired)
        {
            // Already repaired - show a short message if UI available
            if (successMessagePanel != null && successMessageText != null)
            {
                successMessagePanel.SetActive(true);
                successMessageText.text = "Filter already replaced.";
            }
            else
            {
                Debug.Log("Filter already replaced.");
            }
            return;
        }

        if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(filterItemName))
        {
            // Consume item, apply oxygen effect, unlock log, and show success UI
            InventoryManager.Instance.RemoveItem(filterItemName);
            if (LifeSupportManager.Instance != null)
            {
                LifeSupportManager.Instance.IncreaseOxygen(oxygenRestoreAmount);
            }

            if (!string.IsNullOrEmpty(unlockLogID) && GameStateManager.Instance != null)
            {
                GameStateManager.Instance.UnlockLog(unlockLogID);
            }

            if (successMessagePanel != null && successMessageText != null)
            {
                successMessagePanel.SetActive(true);
                successMessageText.text = successMessage;
            }
            else
            {
                Debug.Log(successMessage);
            }

            repaired = true;
        }
        else
        {
            // Show UI prompting that a filter is required, reusing successMessagePanel if desired
            if (successMessagePanel != null && successMessageText != null)
            {
                successMessagePanel.SetActive(true);
                successMessageText.text = $"Requires '{filterItemName}' to repair.";
            }
            else
            {
                Debug.Log($"Requires '{filterItemName}' to repair the filter.");
            }
        }
    }
}

