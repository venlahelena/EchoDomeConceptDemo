using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Controls the dialogue UI: shows speaker name, line text, and dynamically builds choice buttons.
/// Records player choices to the <see cref="GameStateManager"/> and applies any NPC trust deltas.
/// Use <see cref="StartDialogue(DialogueNode)"/> to begin a conversation and <see cref="EndDialogue()"/> to close it.
/// </summary>
/* Editor notes:
 - Assign `dialoguePanel`, `speakerText`, `dialogueText`, a `choicesContainer` and `choiceButtonPrefab`.
 - `choiceButtonPrefab` should contain a TextMeshProUGUI for the label.
 - Use `DialogueNode` assets to author dialogues and wire starting nodes to `DialogueStarter` interactables.
*/
    
public class DialogueRunner : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public Button choiceButtonPrefab;

    private DialogueNode currentNode;

    public void StartDialogue(DialogueNode startingNode)
    {
        dialoguePanel.SetActive(true);
        DisplayNode(startingNode);
    }

    void DisplayNode(DialogueNode node)
    {
        currentNode = node;
        speakerText.text = node.speakerName;
        dialogueText.text = node.line;
        // Reuse existing choice buttons where possible
        int index = 0;
        if (node.choices != null && node.choices.Length > 0)
        {
            foreach (var choice in node.choices)
            {
                Button buttonObj = UIUtils.GetOrCreateChildComponent<Button>(choicesContainer, choiceButtonPrefab.gameObject, index);
                if (buttonObj == null) continue;

                var label = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                if (label != null) label.text = choice.choiceText;

                // Clear old listeners then add a new one that records the choice and displays the next node
                buttonObj.onClick.RemoveAllListeners();
                var capturedChoice = choice; // capture local
                buttonObj.onClick.AddListener(() =>
                {
                    if (!string.IsNullOrEmpty(node.nodeID) && GameStateManager.Instance != null)
                        GameStateManager.Instance.SetDialogueChoice(node.nodeID, capturedChoice.choiceText);

                    // Apply NPC trust effects if configured on the choice
                    if (capturedChoice != null && !string.IsNullOrEmpty(capturedChoice.npcID) && capturedChoice.trustDelta != 0 && GameStateManager.Instance != null)
                    {
                        GameStateManager.Instance.ModifyNpcTrust(capturedChoice.npcID, capturedChoice.trustDelta);
                    }

                    DisplayNode(capturedChoice.nextNode);
                });

                index++;
            }
        }
        else
        {
            Button buttonObj = UIUtils.GetOrCreateChildComponent<Button>(choicesContainer, choiceButtonPrefab.gameObject, index);
            if (buttonObj == null) return;

            var label = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = "Continue";

            buttonObj.onClick.RemoveAllListeners();
            buttonObj.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(node.nodeID) && GameStateManager.Instance != null)
                    GameStateManager.Instance.SetDialogueChoice(node.nodeID, "__visited__");

                EndDialogue();
            });

            index++;
        }

        // Deactivate any extra buttons
        for (int childIndex = index; childIndex < choicesContainer.childCount; childIndex++)
        {
            choicesContainer.GetChild(childIndex).gameObject.SetActive(false);
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}

