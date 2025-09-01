using System.Collections.Generic;
using UnityEngine;

// Simple in-scene room manager: rooms are GameObjects with unique ids. Only the active room is enabled.
public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance;

    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    private string activeRoomID;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    GameObjectUtils.PreserveRoot(this);
    }

    public void RegisterRoom(string id, GameObject roomRoot)
    {
        if (string.IsNullOrEmpty(id) || roomRoot == null) return;
        rooms[id] = roomRoot;
    }

    public void SetActiveRoom(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        if (!rooms.ContainsKey(id))
        {
            Debug.LogWarning("RoomManager: Unknown room id " + id);
            return;
        }

        foreach (var kv in rooms)
        {
            bool active = kv.Key == id;
            if (kv.Value != null)
                kv.Value.SetActive(active);
        }

        activeRoomID = id;
    }

    public string GetActiveRoom() => activeRoomID;
}
