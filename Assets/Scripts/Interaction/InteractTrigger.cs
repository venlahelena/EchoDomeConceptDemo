using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractTrigger : MonoBehaviour
{
    public IInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<IInteractable>() 
                    ?? GetComponentInChildren<IInteractable>() 
                    ?? GetComponentInParent<IInteractable>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InteractionManager.Instance.RegisterInteractable(interactable, this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            InteractionManager.Instance.UnregisterInteractable(interactable);
        }
    }
}