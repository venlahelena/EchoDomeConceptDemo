using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TerminalInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("Optional local list of logs for this terminal. If empty, the global TerminalUIController logs list will be used.")]
    public List<LogEntry> localLogs;

    [Tooltip("Reference to the scene TerminalUIController. If null, the script will attempt to find one.")]
    public TerminalUIController uiController;

    void Start()
    {
        if (uiController == null)
        {
            uiController = FindObjectOfType<TerminalUIController>();
        }
    }

    public void Interact()
    {
        if (uiController == null)
        {
            Debug.LogWarning("TerminalInteractable: No TerminalUIController found in scene.");
            return;
        }

        uiController.ShowForLogs(localLogs);
    }
}
