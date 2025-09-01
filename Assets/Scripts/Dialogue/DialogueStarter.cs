using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueStarter : MonoBehaviour, IInteractable
{
    public DialogueNode startingNode;
    public DialogueNode gatedNode; // Dialogue shown only if oxygen is high enough
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

        if (lifeSupport != null && gatedNode != null)
        {
            if (lifeSupport.oxygenLevel >= requiredOxygenLevel)
            {
                runner.StartDialogue(gatedNode);
                return;
            }
        }

        runner.StartDialogue(startingNode);
    }
}

