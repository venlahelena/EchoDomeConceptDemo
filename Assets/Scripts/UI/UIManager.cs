using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("References")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject); // Prevent duplicates
            return;
        }

    GameObjectUtils.PreserveRoot(this);
    }

    public void ShowMessage(string message)
    {
        if (messagePanel != null && messageText != null)
        {
            messageText.text = message;
            messagePanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("UIManager: messagePanel or messageText not assigned in inspector.");
        }
    }

    public void HideMessage()
    {
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    public bool IsMessageOpen()
    {
        return messagePanel != null && messagePanel.activeSelf;
    }
}
