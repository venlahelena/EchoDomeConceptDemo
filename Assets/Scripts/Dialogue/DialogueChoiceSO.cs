using UnityEngine;

[System.Serializable]
public class DialogueChoiceSO
{
    public string choiceText;
    public DialogueNodeSO nextNode;
    [Tooltip("Optional NPC id that this choice affects (for trust/affinity).")]
    public string npcID;
    [Tooltip("Optional trust delta to apply to the NPC when this choice is selected (can be negative).")]
    public int trustDelta = 0;
}
