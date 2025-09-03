using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Terminal UI controller: shows available logs, handles password gates and oxygen gating (if used).
/// </summary>
/* Editor notes:
 - Assign `logButtonPrefab` to a button prefab with a TextMeshProUGUI child.
 - Hook up `logListParent`, `logDetailPanel`, `logTitleText`, and `logContentText` to the terminal UI.
 - Populate `logs` with `LogEntry` assets in the inspector or leave empty and provide per-terminal logs via TerminalInteractable.
 - Password-protected logs: set `password` on LogEntry; oxygen-gated logs: set `requiredOxygenLevel`.
*/

public class TerminalUIController : MonoBehaviour
{
    public GameObject terminalPanel;
    public UnityEngine.UI.Button logButtonPrefab;
    public Transform logListParent;

    public GameObject logDetailPanel;
    public TextMeshProUGUI logTitleText;
    public TextMeshProUGUI logContentText;

    public Button backButton;
    public Button exitButton;

    [Header("Log Entries")]
    public List<LogEntry> logs;

    public GameObject passwordPanel;
    public TMP_InputField passwordInputField;
    public Button passwordSubmitButton;

    LogEntry pendingPasswordLog;

    void Awake()
    {
        if (passwordPanel != null) passwordPanel.SetActive(false);
        if (logDetailPanel != null) logDetailPanel.SetActive(false);
        if (passwordSubmitButton != null) passwordSubmitButton.onClick.AddListener(OnPasswordSubmit);
    }

    void Start()
    {
        if (backButton != null) backButton.onClick.AddListener(ShowLogList);
        if (exitButton != null) exitButton.onClick.AddListener(CloseTerminal);
        PopulateLogList();
    }

    public void ShowForLogs(List<LogEntry> terminalLogs)
    {
        if (terminalLogs != null && terminalLogs.Count > 0) logs = terminalLogs;
        if (terminalPanel != null) terminalPanel.SetActive(true);
        ShowLogList();
        PopulateLogList();
    }

    void TryShowLogDetail(LogEntry log)
    {
        if (log == null) return;
        if (!string.IsNullOrEmpty(log.password))
        {
            pendingPasswordLog = log;
            if (passwordPanel != null) passwordPanel.SetActive(true);
            if (logListParent != null) logListParent.gameObject.SetActive(false);
            if (logDetailPanel != null) logDetailPanel.SetActive(false);
            return;
        }

        ShowLogDetail(log);
        if (GameStateManager.Instance != null) GameStateManager.Instance.UnlockLog(log.logID);
    }

    void OnPasswordSubmit()
    {
        if (pendingPasswordLog == null || passwordInputField == null) return;
        if (passwordInputField.text == pendingPasswordLog.password)
        {
            ShowLogDetail(pendingPasswordLog);
            if (GameStateManager.Instance != null) GameStateManager.Instance.UnlockLog(pendingPasswordLog.logID);
            pendingPasswordLog = null;
            passwordInputField.text = string.Empty;
            if (passwordPanel != null) passwordPanel.SetActive(false);
        }
        else
        {
            passwordInputField.text = string.Empty;
        }
    }

    public void ShowLogDetail(LogEntry log)
    {
        if (log == null) return;
        if (logTitleText != null) logTitleText.text = log.title;
        if (logContentText != null) logContentText.text = log.content;
        if (logDetailPanel != null) logDetailPanel.SetActive(true);
        if (logListParent != null) logListParent.gameObject.SetActive(false);
        if (passwordPanel != null) passwordPanel.SetActive(false);
    }

    public void ShowLogList()
    {
        if (logDetailPanel != null) logDetailPanel.SetActive(false);
        if (passwordPanel != null) passwordPanel.SetActive(false);
        if (logListParent != null) logListParent.gameObject.SetActive(true);
    }

    public void CloseTerminal()
    {
        if (terminalPanel != null) terminalPanel.SetActive(false);
    }

    void PopulateLogList()
    {
        if (logListParent == null || logButtonPrefab == null) return;

        // Clear existing children
        for (int i = logListParent.childCount - 1; i >= 0; i--)
        {
            var child = logListParent.GetChild(i).gameObject;
            if (Application.isPlaying) Destroy(child);
            else DestroyImmediate(child);
        }

        if (logs == null) return;

        foreach (var log in logs)
        {
            if (log == null) continue;
            var btn = Instantiate(logButtonPrefab, logListParent);
            btn.gameObject.SetActive(true);
            var text = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = log.title;
            // Determine if this log should be interactable based on required oxygen or prior unlock
            bool isUnlocked = false;
            if (GameStateManager.Instance != null && !string.IsNullOrEmpty(log.logID))
                isUnlocked = GameStateManager.Instance.IsLogUnlocked(log.logID);

            bool oxygenLocked = false;
            if (log.requiredOxygenLevel > 0f && LifeSupportManager.Instance != null)
            {
                if (LifeSupportManager.Instance.oxygenLevel < log.requiredOxygenLevel)
                    oxygenLocked = true;
            }

            // If the log is unlocked previously, ignore oxygen locking
            if (isUnlocked)
                oxygenLocked = false;

            if (oxygenLocked)
            {
                // show locked state and disable the button
                if (text != null)
                    text.text = $"{log.title} (Locked: requires {log.requiredOxygenLevel:F0}% O₂)";
                btn.interactable = false;
            }
            else
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => TryShowLogDetail(log));
            }
        }
    }
}