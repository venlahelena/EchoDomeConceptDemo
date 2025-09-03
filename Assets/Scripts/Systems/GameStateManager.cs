using System;
using System.Collections.Generic;
using System.IO;
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

    // Internal runtime state (kept private but with clearer names)
    private HashSet<string> unlockedLogIds = new HashSet<string>();
    private Dictionary<string, int> plantStateById = new Dictionary<string, int>();
    private Dictionary<string, string> dialogueChoiceByNodeId = new Dictionary<string, string>();
    private Dictionary<string, int> npcTrustById = new Dictionary<string, int>();

    // Full path to the save file on disk
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "gamestate.json");

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
        try
        {
            GameStateData data = new GameStateData();
            // Keep the serialized shape compatible with previous saves by populating GameStateData fields
            data.unlockedLogs = new List<string>(unlockedLogIds);
            data.plantStates = new List<PlantStateEntry>();
            foreach (var kv in plantStateById)
            {
                data.plantStates.Add(new PlantStateEntry { plantID = kv.Key, healthValue = kv.Value });
            }
            data.dialogueChoices = new List<DialogueChoiceEntry>();
            foreach (var kv in dialogueChoiceByNodeId)
            {
                data.dialogueChoices.Add(new DialogueChoiceEntry { nodeID = kv.Key, choiceText = kv.Value });
            }
            data.npcTrust = new List<NpcTrustEntry>();
            foreach (var kv in npcTrustById)
            {
                data.npcTrust.Add(new NpcTrustEntry { npcID = kv.Key, trustValue = kv.Value });
            }

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(saveFile, json);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("GameStateManager: Failed to save game state: " + ex.Message);
        }
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(saveFile)) return;
            string json = File.ReadAllText(saveFile);
            GameStateData data = JsonUtility.FromJson<GameStateData>(json);
            if (data == null) return;

            unlockedLogIds = new HashSet<string>(data.unlockedLogs ?? new List<string>());
            plantStateById = new Dictionary<string, int>();
            if (data.plantStates != null)
            {
                foreach (var plantEntry in data.plantStates)
                {
                    if (!string.IsNullOrEmpty(plantEntry.plantID))
                        plantStateById[plantEntry.plantID] = plantEntry.healthValue;
                }
            }
            dialogueChoiceByNodeId = new Dictionary<string, string>();
            if (data.dialogueChoices != null)
            {
                foreach (var dialogueEntry in data.dialogueChoices)
                {
                    if (!string.IsNullOrEmpty(dialogueEntry.nodeID))
                        dialogueChoiceByNodeId[dialogueEntry.nodeID] = dialogueEntry.choiceText;
                }
            }
            npcTrustById = new Dictionary<string, int>();
            if (data.npcTrust != null)
            {
                foreach (var trustEntry in data.npcTrust)
                {
                    if (!string.IsNullOrEmpty(trustEntry.npcID))
                        npcTrustById[trustEntry.npcID] = trustEntry.trustValue;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("GameStateManager: Failed to load game state: " + ex.Message);
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
