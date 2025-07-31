using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionMessage : MonoBehaviour, IInteractable
{
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    [TextArea] public string message;

    public void Interact()
    {
        if (messagePanel != null && messageText != null)
        {
            messagePanel.SetActive(true);
            messageText.text = message;
        }
    }
}
