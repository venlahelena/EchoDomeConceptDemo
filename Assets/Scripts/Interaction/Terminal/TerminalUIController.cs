using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TerminalUIController : MonoBehaviour
{
    public GameObject terminalPanel;
    public GameObject logButtonPrefab;
    public Transform logListParent;

    public GameObject logDetailPanel;
    public TextMeshProUGUI logTitleText;
    public TextMeshProUGUI logContentText;

    public Button backButton;
    public Button exitButton;

    [Header("Log Entries")]
    public List<LogEntry> logs;

    // Cached references
    LifeSupportManager lifeSupport;

    void Start()
    {
    // Cache managers and initialize UI
    lifeSupport = LifeSupportManager.Instance;
    PopulateLogList();
    if (logDetailPanel != null) logDetailPanel.SetActive(false);

    if (backButton != null) backButton.onClick.AddListener(() => ShowLogList());
    if (exitButton != null) exitButton.onClick.AddListener(() => CloseTerminal());
    }

    // ...existing code...

    /// <summary>
    /// Opens this terminal UI and populates it with the provided logs (used by TerminalInteractable on prefabs).
    /// If you want a terminal to use its own per-prefab logs, call this method from the interactable component.
    /// </summary>
    public void ShowForLogs(List<LogEntry> terminalLogs)
    {
        // Only override the controller's logs if the provided list is non-null and contains entries.
        // This preserves the global/default logs when a terminal's `localLogs` is left empty in the inspector.
        if (terminalLogs != null && terminalLogs.Count > 0)
            this.logs = terminalLogs;

    // Ensure UI is visible and populated
    if (terminalPanel != null) terminalPanel.SetActive(true);
    PopulateLogList();
        if (logDetailPanel != null) logDetailPanel.SetActive(false);
    }

    public GameObject passwordPanel;
    public TMP_InputField passwordInputField;
    public Button passwordSubmitButton;
    private LogEntry pendingPasswordLog;

    void Awake()
    {
        if (passwordPanel != null)
            passwordPanel.SetActive(false);
        if (passwordSubmitButton != null)
            passwordSubmitButton.onClick.AddListener(OnPasswordSubmit);
    }

    void TryShowLogDetail(LogEntry log)
    {
        if (!string.IsNullOrEmpty(log.password))
        {
            pendingPasswordLog = log;
            passwordPanel.SetActive(true);
            logDetailPanel.SetActive(false);
            logListParent.gameObject.SetActive(false);
        }
        else
        {
            ShowLogDetail(log);
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.UnlockLog(log.logID);
        }
    }

    void OnPasswordSubmit()
    {
        if (pendingPasswordLog != null && passwordInputField != null)
        {
            if (passwordInputField.text == pendingPasswordLog.password)
            {
                ShowLogDetail(pendingPasswordLog);
                if (GameStateManager.Instance != null)
                    GameStateManager.Instance.UnlockLog(pendingPasswordLog.logID);
                if (passwordPanel != null) passwordPanel.SetActive(false);
                passwordInputField.text = "";
                pendingPasswordLog = null;
            }
            else
            {
                // Optionally show error message
                passwordInputField.text = "";
            }
        }
    }

    public void ShowLogDetail(LogEntry log)
    {
        if (logTitleText != null) logTitleText.text = log.title;
        if (logContentText != null) logContentText.text = log.content;

        if (logDetailPanel != null) logDetailPanel.SetActive(true);
        if (logListParent != null) logListParent.gameObject.SetActive(false);
        if (passwordPanel != null) passwordPanel.SetActive(false);
    }

    public void ShowLogList()
    {
        if (logDetailPanel != null) logDetailPanel.SetActive(false);
        if (logListParent != null) logListParent.gameObject.SetActive(true);
    }

    public void CloseTerminal()
    {
        if (terminalPanel != null) terminalPanel.SetActive(false);
    }

    void PopulateLogList()
    {
        if (logButtonPrefab == null)
        {
            Debug.LogError("TerminalUIController: logButtonPrefab is not assigned.");
            return;
        }

        if (logListParent == null)
        {
            Debug.LogError("TerminalUIController: logListParent is not assigned.", this);
            return;
        }

        if (logs == null || logs.Count == 0)
        {
            Debug.LogWarning($"TerminalUIController.PopulateLogList: no logs to show (logs is null or empty). logsCount={(logs==null?0:logs.Count)}", this);
        }

        int index = 0;
        for (int i = 0; i < (logs != null ? logs.Count : 0); i++)
        {
            var log = logs[i];
            if (log == null)
            {
                Debug.LogWarning($"TerminalUIController: encountered null LogEntry in logs list at index {i}", this);
                continue;
            }

            // Oxygen-level gating removed: list all logs regardless of LifeSupport oxygen.
            bool unlocked = GameStateManager.Instance != null && GameStateManager.Instance.IsLogUnlocked(log.logID);

            // passworded logs are still shown but will prompt when opened; report that for debugging
            if (!string.IsNullOrEmpty(log.password))
            {
                Debug.Log($"TerminalUIController: log '{log.title}' (id={log.logID}) is password protected and will prompt on open.", this);
            }

            var btn = UIUtils.GetOrCreateChildComponent<Button>(logListParent, logButtonPrefab, index);
            if (btn == null)
            {
                string prefabName = logButtonPrefab != null ? logButtonPrefab.name : "<null prefab>";
                Debug.LogError($"TerminalUIController: failed to get Button from prefab '{prefabName}' — check that the prefab contains a UnityEngine.UI.Button component.", this);
                continue;
            }

            var textComponent = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
                textComponent.text = log.title;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => TryShowLogDetail(log));

            index++;
        }

        for (int childIndex = index; childIndex < logListParent.childCount; childIndex++)
        {
            logListParent.GetChild(childIndex).gameObject.SetActive(false);
        }
    }
}