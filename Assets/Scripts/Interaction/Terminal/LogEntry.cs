using UnityEngine;

[CreateAssetMenu(fileName = "NewLogEntry", menuName = "Terminal/Log Entry")]
public class LogEntry : ScriptableObject
{
    public string logID;
    public string title;
    [TextArea(5, 20)] public string content;
    public float requiredOxygenLevel = 0f; // If > 0, log is gated by oxygen
    public string password; // If not empty, log is password protected
}