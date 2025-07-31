using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager UIManagerInstance;

    [Header("References")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    private void Awake()
    {
        if (UIManagerInstance == null)
            UIManagerInstance = this;
        else
            Destroy(gameObject); // Prevent duplicates
    }

    public void ShowMessage(string message)
    {
        if (messagePanel != null && messageText != null)
        {
            messageText.text = message;
            messagePanel.SetActive(true);
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
