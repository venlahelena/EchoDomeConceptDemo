using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public DialogueNode nextNode;
    [Tooltip("Optional NPC id that this choice affects (for trust/affinity).")]
    public string npcID;
    [Tooltip("Optional trust delta to apply to the NPC when this choice is selected (can be negative).")]
    public int trustDelta = 0;
}

