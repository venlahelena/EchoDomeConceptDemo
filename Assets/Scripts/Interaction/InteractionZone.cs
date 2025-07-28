using UnityEngine;
using TMPro;

public class InteractionZone : MonoBehaviour, IInteractable
{
    [Header("UI References")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;

    [Header("Message Content")]
    [TextArea]
    public string message = "Default message";

    public void Interact()
    {
        bool isActive = messagePanel.activeSelf;
        messagePanel.SetActive(!isActive);

        if (!isActive && messageText != null)
            messageText.text = message;
    }
}