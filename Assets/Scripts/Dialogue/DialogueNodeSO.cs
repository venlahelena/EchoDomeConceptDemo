using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "Dialogue/Node")]
public class DialogueNodeSO : ScriptableObject
{
    [Tooltip("Optional ID used to record player choice/visit in save data")]
    public string nodeID;
    public string speakerName;
    [TextArea(3, 10)]
    public string line;
    public DialogueChoiceSO[] choices;
}
