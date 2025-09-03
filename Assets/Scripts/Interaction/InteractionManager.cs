using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains a registry of active interactables in the scene, displays optional hover prompts,
/// and routes click interactions to the nearest <see cref="IInteractable"/> within range.
/// Interactables should register/unregister themselves via <see cref="RegisterInteractable"/> and <see cref="UnregisterInteractable"/>.
/// </summary>

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    // Simple list of interactables that register/unregister on enable/disable
    private readonly List<MonoBehaviour> interactables = new List<MonoBehaviour>();

    [Header("Prompt UI")]
    [Tooltip("Optional UI GameObject (world-space or screen-space) to show when an interactable is hovered")]
    public GameObject interactPromptPrefab;
    GameObject promptInstance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Update()
    {
        UpdateHoverPrompt();
    }

    void UpdateHoverPrompt()
    {
        if (interactPromptPrefab == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        MonoBehaviour nearest = FindNearestInteractable(mouseWorld, 1.0f);
        if (nearest != null)
        {
            if (promptInstance == null)
                promptInstance = Instantiate(interactPromptPrefab);

            // position promptInstance at the interactable position (if it has a transform), otherwise at mouse
            promptInstance.transform.position = nearest.transform.position + Vector3.up * 0.5f;
            if (!promptInstance.activeSelf) promptInstance.SetActive(true);
        }
        else
        {
            if (promptInstance != null && promptInstance.activeSelf)
                promptInstance.SetActive(false);
        }
    }

    // Register an interactable (call from OnEnable)
    public void RegisterInteractable(MonoBehaviour interactable)
    {
        if (!interactables.Contains(interactable)) interactables.Add(interactable);
    }

    // Unregister (call from OnDisable)
    public void UnregisterInteractable(MonoBehaviour interactable)
    {
        if (interactables.Contains(interactable)) interactables.Remove(interactable);
    }

    // Find the nearest interactable (within maxDistance). Returns null if none.
    MonoBehaviour FindNearestInteractable(Vector3 worldPoint, float maxDistance)
    {
        MonoBehaviour best = null;
        float bestDist = maxDistance;

        foreach (var interactable in interactables)
        {
            if (interactable == null) continue;
            float distance = Vector3.Distance(interactable.transform.position, worldPoint);
            if (distance <= bestDist)
            {
                best = interactable;
                bestDist = distance;
            }
        }

        return best;
    }

    // Called by PlayerMovement or input code when the player clicks somewhere.
    // Returns true if an interactable handled the click.
    public bool ClickInteractAt(Vector3 worldPoint)
    {
        MonoBehaviour nearest = FindNearestInteractable(worldPoint, 1.5f);
        if (nearest != null && nearest is IInteractable)
        {
            ((IInteractable)nearest).Interact();
            return true;
        }

        return false;
    }
}
