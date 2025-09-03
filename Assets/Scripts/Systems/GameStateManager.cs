using System;
using System.Collections.Generic;
// using System.IO; // file IO disabled until a save system is implemented
using UnityEngine;

[Serializable]
public class PlantStateEntry
{
    public string plantID;
    public int healthValue;
}

[Serializable]
public class GameStateData
{
    public List<string> unlockedLogs = new List<string>();
    public List<PlantStateEntry> plantStates = new List<PlantStateEntry>();
    public List<DialogueChoiceEntry> dialogueChoices = new List<DialogueChoiceEntry>();
    public List<NpcTrustEntry> npcTrust = new List<NpcTrustEntry>();
}

[Serializable]
public class DialogueChoiceEntry
{
    public string nodeID;
    public string choiceText;
}

[Serializable]
public class NpcTrustEntry
{
    public string npcID;
    public int trustValue;
}

/// <summary>
/// Singleton that manages persistent game state: unlocked logs, plant states, dialogue choices, and NPC trust.
/// Provides simple Save/Load using JsonUtility to Application.persistentDataPath.
/// Exposes APIs to modify state and query persisted values. Fires <c>OnLogUnlocked</c> when logs are unlocked.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    // Event fired when a log is unlocked. Argument: logID
    public event System.Action<string> OnLogUnlocked;
    // Event fired when a dialogue node choice or visit is recorded. Arguments: nodeID, choiceText
    public event System.Action<string, string> OnDialogueChoiceRecorded;

    // Internal runtime state (kept private but with clearer names)
    private HashSet<string> unlockedLogIds = new HashSet<string>();
    private Dictionary<string, int> plantStateById = new Dictionary<string, int>();
    private Dictionary<string, string> dialogueChoiceByNodeId = new Dictionary<string, string>();
    private Dictionary<string, int> npcTrustById = new Dictionary<string, int>();

    // Save system is currently disabled. Persistent save/load will be added later.

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    GameObjectUtils.PreserveRoot(this);

        Load();
    }

    public bool IsLogUnlocked(string logID)
    {
    if (string.IsNullOrEmpty(logID)) return false;
    return unlockedLogIds.Contains(logID);
    }

    public void UnlockLog(string logID)
    {
    if (string.IsNullOrEmpty(logID)) return;
    if (unlockedLogIds.Add(logID))
        {
            Save();
            try
            {
                OnLogUnlocked?.Invoke(logID);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("GameStateManager: OnLogUnlocked handler threw: " + ex.Message);
            }
        }
    }

    public void SetPlantState(string plantID, int healthValue)
    {
    if (string.IsNullOrEmpty(plantID)) return;
    plantStateById[plantID] = healthValue;
        Save();
    }

    public void SetDialogueChoice(string nodeID, string choiceText)
    {
    if (string.IsNullOrEmpty(nodeID)) return;
    dialogueChoiceByNodeId[nodeID] = choiceText;
        Save();
        try
        {
            OnDialogueChoiceRecorded?.Invoke(nodeID, choiceText);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("GameStateManager: OnDialogueChoiceRecorded handler threw: " + ex.Message);
        }
    }

    // NPC trust APIs
    public void SetNpcTrust(string npcID, int trustValue)
    {
    if (string.IsNullOrEmpty(npcID)) return;
    npcTrustById[npcID] = trustValue;
        Save();
    }

    public void ModifyNpcTrust(string npcID, int delta)
    {
    if (string.IsNullOrEmpty(npcID)) return;
    int current = 0;
    npcTrustById.TryGetValue(npcID, out current);
    npcTrustById[npcID] = current + delta;
        Save();
    }

    public bool TryGetNpcTrust(string npcID, out int trustValue)
    {
    trustValue = 0;
    if (string.IsNullOrEmpty(npcID)) return false;
    return npcTrustById.TryGetValue(npcID, out trustValue);
    }

    public bool TryGetDialogueChoice(string nodeID, out string choiceText)
    {
    choiceText = null;
    if (string.IsNullOrEmpty(nodeID)) return false;
    return dialogueChoiceByNodeId.TryGetValue(nodeID, out choiceText);
    }

    public bool TryGetPlantState(string plantID, out int healthValue)
    {
    healthValue = -1;
    if (string.IsNullOrEmpty(plantID)) return false;
    return plantStateById.TryGetValue(plantID, out healthValue);
    }

    public void Save()
    {
        // Save disabled: no persistent save system yet.
        // This method is intentionally a no-op until a save system is introduced.
        return;
    }

    public void Load()
    {
        // Load disabled: no persistent save system yet.
        // Runtime state will remain initialized from defaults.
        return;
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
