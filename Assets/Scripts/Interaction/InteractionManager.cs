using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    public GameObject interactPrompt;

    private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    private IInteractable currentInteractable;

    private Transform playerTransform;
    private Vector2 playerFacingDirection = Vector2.right; // Set according to your player facing logic

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (nearbyInteractables.Count == 0)
        {
            HidePrompt();
            currentInteractable = null;
            return;
        }

        // Find closest interactable, optionally check facing direction
        IInteractable best = null;
        float bestDistance = Mathf.Infinity;

        foreach (var interactable in nearbyInteractables)
        {
            var obj = (interactable as MonoBehaviour)?.transform;
            if (obj == null) continue;

            float dist = Vector2.Distance(playerTransform.position, obj.position);

            // Optional: check facing direction angle
            Vector2 toObj = obj.position - playerTransform.position;
            float angle = Vector2.Angle(playerFacingDirection, toObj);

            if (angle < 60f && dist < bestDistance)
            {
                best = interactable;
                bestDistance = dist;
            }
        }

        if (best != currentInteractable)
        {
            currentInteractable = best;
            ShowPrompt();
        }

        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            if (UIManager.UIManagerInstance != null && UIManager.UIManagerInstance.IsMessageOpen())
            {
                UIManager.UIManagerInstance.HideMessage();
            }
            else
            {
                currentInteractable.Interact();
            }
        }
    }

    public void RegisterInteractable(IInteractable interactable, InteractTrigger trigger)
    {
        if (!nearbyInteractables.Contains(interactable))
            nearbyInteractables.Add(interactable);
    }

    public void UnregisterInteractable(IInteractable interactable)
    {
        nearbyInteractables.Remove(interactable);

        if (interactable == currentInteractable)
        {
            currentInteractable = null;
            HidePrompt();
        }
    }

    private void ShowPrompt()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(true);
    }

    private void HidePrompt()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }
}

