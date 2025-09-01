using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RoomTransition : MonoBehaviour
{
    [Tooltip("Target room id to activate when this transition is used")]
    public string targetRoomID;

    [Tooltip("Optional position in the target room where the player will be placed")]
    public Transform targetSpawnPoint;

    void Start()
    {
        var col = GetComponent<Collider2D>();
        if (!col.isTrigger)
            col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.SetActiveRoom(targetRoomID);
                if (targetSpawnPoint != null)
                {
                    var player = other.transform;
                    player.position = targetSpawnPoint.position;
                }
            }
        }
    }
}
