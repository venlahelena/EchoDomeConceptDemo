using UnityEngine;

[CreateAssetMenu(fileName = "NewLogEntry", menuName = "Terminal/Log Entry")]
public class LogEntry : ScriptableObject
{
    public string logID;
    public string title;
    [TextArea(5, 20)] public string content;
}