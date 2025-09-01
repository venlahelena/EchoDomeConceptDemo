using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    [Tooltip("Optional ID used to record player choice/visit in save data")]
    public string nodeID;
    public string speakerName;
    [TextArea(3, 10)]
    public string line;
    public DialogueChoice[] choices;
}