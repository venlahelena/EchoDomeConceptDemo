using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Starts a dialogue when interacted with. Can optionally gate a special dialogue node behind an oxygen threshold.
/// </summary>
/* Editor notes:
 - Assign `startingNode` and optionally `gatedNode` in the inspector.
 - Set `requiredOxygenLevel` to gate the `gatedNode` behind LifeSupport oxygen.
 - Ensure a `DialogueRunner` exists in the scene (auto-located at Start if not assigned).
*/

public class DialogueStarter : MonoBehaviour, IInteractable
{
    public DialogueNode startingNode;
    public DialogueNode gatedNode; // Dialogue shown only if oxygen is high enough
    public DialogueNodeSO startingNodeSO;
    public DialogueNodeSO gatedNodeSO; // prefer SOs when available
    public float requiredOxygenLevel = 60f; // Example threshold for gated dialogue

    LifeSupportManager lifeSupport;
    DialogueRunner runner;

    void Start()
    {
        // Prefer singleton instance if available
        lifeSupport = LifeSupportManager.Instance;
        if (runner == null)
            runner = FindObjectOfType<DialogueRunner>();
    }

    public void Interact()
    {
        if (runner == null)
        {
            Debug.LogWarning("DialogueStarter: No DialogueRunner found in scene.");
            return;
        }

        // Prefer ScriptableObjects when provided (designer-friendly assets)
        if (lifeSupport != null && gatedNodeSO != null)
        {
            if (lifeSupport.oxygenLevel >= requiredOxygenLevel)
            {
                runner.StartDialogue(gatedNodeSO);
                return;
            }
        }

        if (startingNodeSO != null)
        {
            runner.StartDialogue(startingNodeSO);
            return;
        }

        if (lifeSupport != null && gatedNode != null)
        {
            if (lifeSupport.oxygenLevel >= requiredOxygenLevel)
            {
                runner.StartDialogue(gatedNode);
                return;
            }
        }

        if (startingNode != null)
            runner.StartDialogue(startingNode);
    }
}

