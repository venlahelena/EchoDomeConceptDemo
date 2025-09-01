using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    public TMPro.TextMeshProUGUI objectiveText;

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

        // Subscribe to log unlocks to auto-complete objectives if they match
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnLogUnlocked += OnLogUnlocked;
    }

    void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnLogUnlocked -= OnLogUnlocked;
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

    public Objective GetObjective(string id) => objectives.Find(o => o.id == id);

    public List<Objective> GetAllObjectives() => objectives;

    private void UpdateUI()
    {
        if (objectiveText == null) return;

        var active = CurrentObjective;
        if (active != null)
        {
            objectiveText.text = $"Objective: {active.title} ({objectives.Count(o => !o.completed)} remaining)";
        }
        else
        {
            objectiveText.text = "Objective: None";
        }
    }
}

