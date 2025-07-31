using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    private IInteractable interactable;
    private bool playerInRange;
    public GameObject interactPrompt;

    void Awake()
    {
        // Tries to find IInteractable on self, children, or parent
        interactable = GetComponent<IInteractable>() 
                    ?? GetComponentInChildren<IInteractable>() 
                    ?? GetComponentInParent<IInteractable>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // If panel is open, close it
            if (UIManager.UIManagerInstance != null && UIManager.UIManagerInstance.IsMessageOpen())
            {
                UIManager.UIManagerInstance.HideMessage();
                return;
            }

            // Otherwise interact as normal
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
}