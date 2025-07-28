using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableObject : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;


    [Header("Message Content")]
    [TextArea]
    public string message = "Default terminal message.";

    private void OnMouseDown()
    {
        if (messagePanel != null && messageText != null)
        {
            messagePanel.SetActive(true);
            messageText.text = message;
        }
        else
        {
            Debug.LogWarning("UI elements not assigned on " + gameObject.name);
        }
    }
}