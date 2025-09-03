using UnityEngine;

[CreateAssetMenu(fileName = "Objective", menuName = "Objectives/Objective")]
public class ObjectiveSO : ScriptableObject
{
    public string id;
    public string title;
    [TextArea]
    public string description;
    [Tooltip("Optional: if set, this objective will auto-complete when a log with this ID is unlocked.")]
    public string autoCompleteOnLogID;
}
