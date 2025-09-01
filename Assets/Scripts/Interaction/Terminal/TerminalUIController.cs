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

    if (backButton != null) backButton.onClick.AddListener(ShowLogList);
    if (exitButton != null) exitButton.onClick.AddListener(CloseTerminal);
    }

    void PopulateLogList()
    {
        if (logButtonPrefab == null)
        {
            Debug.LogError("TerminalUIController: logButtonPrefab is not assigned.");
            return;
        }

        int index = 0;
        foreach (var log in logs)
        {
            bool canShow = true;
            // Show if unlocked in GameState OR if oxygen requirement is met (or no requirement)
            if (GameStateManager.Instance != null && GameStateManager.Instance.IsLogUnlocked(log.logID))
            {
                canShow = true;
            }
            else if (log.requiredOxygenLevel > 0f && lifeSupport != null)
            {
                if (lifeSupport.oxygenLevel < log.requiredOxygenLevel)
                    canShow = false;
            }
            if (!canShow) continue;

            var btn = UIUtils.GetOrCreateChildComponent<Button>(logListParent, logButtonPrefab, index);
            if (btn == null) continue;

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
                passwordPanel.SetActive(false);
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
        logTitleText.text = log.title;
        logContentText.text = log.content;

        logDetailPanel.SetActive(true);
        logListParent.gameObject.SetActive(false);
        if (passwordPanel != null)
            passwordPanel.SetActive(false);
    }

    public void ShowLogList()
    {
        logDetailPanel.SetActive(false);
        logListParent.gameObject.SetActive(true);
    }

    public void CloseTerminal()
    {
        terminalPanel.SetActive(false);
    }
}