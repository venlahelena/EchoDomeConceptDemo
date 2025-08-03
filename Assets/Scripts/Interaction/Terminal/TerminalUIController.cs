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

    void Start()
    {
        PopulateLogList();
        logDetailPanel.SetActive(false);

        backButton.onClick.AddListener(ShowLogList);
        exitButton.onClick.AddListener(CloseTerminal);
    }

    void PopulateLogList()
    {
        foreach (Transform child in logListParent)
            Destroy(child.gameObject);

        foreach (var log in logs)
        {
            GameObject buttonObj = Instantiate(logButtonPrefab, logListParent);
            var textComponent = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
                textComponent.text = log.title;

            buttonObj.GetComponent<Button>().onClick.AddListener(() => ShowLogDetail(log));
        }
    }

    public void ShowLogDetail(LogEntry log)
    {
        logTitleText.text = log.title;
        logContentText.text = log.content;

        logDetailPanel.SetActive(true);
        logListParent.gameObject.SetActive(false);
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