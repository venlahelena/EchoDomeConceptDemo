using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    public TMPro.TextMeshProUGUI objectiveText;
    [Header("Direct Objective assignment (optional)")]
    [Tooltip("Optional: assign ObjectiveSO assets directly here instead of using an ObjectiveCollectionSO/ObjectiveLoader.")]
    public ObjectiveSO[] initialObjectives;

    // Simple objective model
    [Serializable]
    public class Objective
    {
        public string id;
        public string title;
        public string description;
        public bool completed = false;
        public string timestamp; // ISO string when completed or added
    }
    public TMPro.TextMeshProUGUI objectiveDetailText;

    // Ordered list of objectives
    private List<Objective> objectives = new List<Objective>();

    // Quick access to currently active objective (first incomplete)
    public Objective CurrentObjective => objectives.FirstOrDefault(o => !o.completed);

    // Events
    public event Action<Objective> OnObjectiveAdded;
    public event Action<Objective> OnObjectiveCompleted;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        GameObjectUtils.PreserveRoot(this);

        // Subscribe to log unlocks and dialogue events to auto-complete objectives if they match
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnLogUnlocked += OnLogUnlocked;
            GameStateManager.Instance.OnDialogueChoiceRecorded += OnDialogueChoiceRecorded;
        }
    }

    void Start()
    {
        // If designers assigned ObjectiveSO assets directly on this component, load them into the runtime list.
        if (initialObjectives != null && initialObjectives.Length > 0)
        {
            foreach (var so in initialObjectives)
            {
                if (so == null) continue;
                AddObjective(string.IsNullOrEmpty(so.id) ? null : so.id, so.title, so.description);
            }
        }
        UpdateUI();
    }

    void OnDestroy()
    {
        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.OnLogUnlocked -= OnLogUnlocked;
            GameStateManager.Instance.OnDialogueChoiceRecorded -= OnDialogueChoiceRecorded;
        }
    }

    void OnDialogueChoiceRecorded(string nodeID, string choiceText)
    {
        // If there's an objective with the same id as the dialogue node, mark it complete
        var obj = objectives.Find(o => o.id == nodeID);
        if (obj != null && !obj.completed)
        {
            CompleteObjective(obj.id);
        }
    }

    void OnLogUnlocked(string logID)
    {
        // If there's an objective with the same id as the log, mark it complete
        var obj = objectives.Find(o => o.id == logID);
        if (obj != null && !obj.completed)
        {
            CompleteObjective(obj.id);
        }
    }


    public void SetObjective(string newObjective)
    {
        // Backwards-compat: replace active objective with a simple string
        objectives.Clear();
        objectives.Add(new Objective { id = "legacy", title = newObjective, description = newObjective, completed = false });
        UpdateUI();
    }

    public void AddObjective(string id, string title, string description)
    {
        if (string.IsNullOrEmpty(id)) id = Guid.NewGuid().ToString();
        if (objectives.Exists(o => o.id == id)) return;
        var obj = new Objective { id = id, title = title, description = description, completed = false, timestamp = System.DateTime.UtcNow.ToString("o") };
        objectives.Add(obj);
        OnObjectiveAdded?.Invoke(obj);
        UpdateUI();
    }

    public void CompleteObjective(string id)
    {
        var obj = objectives.Find(o => o.id == id);
        if (obj == null) return;
        if (obj.completed) return;
        obj.completed = true;
        obj.timestamp = System.DateTime.UtcNow.ToString("o");
        OnObjectiveCompleted?.Invoke(obj);
        UpdateUI();
    }

    // Set a specific objective active (mark all others incomplete and move it to front)
    public void SetActiveObjective(string id)
    {
        var obj = objectives.Find(o => o.id == id);
        if (obj == null) return;

        // Mark all objectives incomplete
        for (int i = 0; i < objectives.Count; i++)
            objectives[i].completed = false;

        // Move the chosen objective to the front and clear its timestamp
        objectives.Remove(obj);
        obj.completed = false;
        obj.timestamp = null;
        objectives.Insert(0, obj);

        UpdateUI();
    }

    // Helper: try to complete an objective by id if present
    void TryCompleteById(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        var obj = objectives.Find(o => o.id == id);
        if (obj != null && !obj.completed)
            CompleteObjective(obj.id);
    }

    public Objective GetObjective(string id) => objectives.Find(o => o.id == id);

    public List<Objective> GetAllObjectives() => objectives;

    private void UpdateUI()
    {
    if (objectiveText == null) return;

    var active = CurrentObjective;
        if (active != null)
        {
            objectiveText.text = $"Objective: {active.title} ({objectives.Count(o => !o.completed)} remaining)";
            if (objectiveDetailText != null) objectiveDetailText.text = active.description;
        }
        else
        {
            objectiveText.text = "Objective: None";
            if (objectiveDetailText != null) objectiveDetailText.text = "";
        }
    }
}

