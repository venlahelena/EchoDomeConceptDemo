using UnityEngine;

public class InteractionImage : MonoBehaviour, IInteractable
{
    public GameObject imagePanel;

    public void Interact()
    {
        if (imagePanel != null)
        {
            imagePanel.SetActive(true);
        }
    }
}