using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class NpcTrustController : MonoBehaviour
{
    [Tooltip("Unique NPC id used for saving/tracking trust")]
    public string npcID;

    [Header("Trust Events")]
    public UnityEvent onTrustIncreased;
    public UnityEvent onTrustDecreased;

    void Start()
    {
        // Ensure we have current trust if available
        if (!string.IsNullOrEmpty(npcID) && GameStateManager.Instance != null)
        {
            int trust;
            if (GameStateManager.Instance.TryGetNpcTrust(npcID, out trust))
            {
                // Could initialize visual state based on trust
            }
        }
    }

    public void ModifyTrust(int delta)
    {
        if (string.IsNullOrEmpty(npcID) || GameStateManager.Instance == null) return;
        GameStateManager.Instance.ModifyNpcTrust(npcID, delta);
        if (delta > 0) onTrustIncreased?.Invoke();
        else if (delta < 0) onTrustDecreased?.Invoke();
    }

    public void SetTrust(int value)
    {
        if (string.IsNullOrEmpty(npcID) || GameStateManager.Instance == null) return;
        GameStateManager.Instance.SetNpcTrust(npcID, value);
    }
}
