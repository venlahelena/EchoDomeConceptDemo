using UnityEngine;

/// <summary>
/// Handles player movement and click-to-move interactions.
/// Supports moving within a 2D walkable PolygonCollider2D area and dispatching interactions when target reached.
/// Use <see cref="MoveToWithInteraction(Vector3, IInteractable)"/> to move the player and trigger an interaction on arrival.
/// </summary>
/// 
/* Editor notes:
 - Assign `walkArea` to a PolygonCollider2D representing where the player can walk.
 - Set `feetPosition` to a child transform at the player's feet for accurate overlap checks.
 - Tune `moveSpeed` for desired pacing; in Play mode, click to move and verify player stops at the target.
*/
    
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public PolygonCollider2D walkArea;
    public Transform feetPosition;

    private Vector3 targetPosition;
    private bool isMoving = false;

    private IInteractable targetInteraction = null;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        HandleClickToMove();

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                isMoving = false;

                if (targetInteraction != null)
                {
                    targetInteraction.Interact();
                    targetInteraction = null;
                }
            }
        }
    }

    void HandleClickToMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickPos.z = transform.position.z;

            // First, let the InteractionManager try to handle this click (nearest interactable).
            if (InteractionManager.Instance != null)
            {
                bool handled = InteractionManager.Instance.ClickInteractAt(clickPos);
                if (handled)
                {
                    // InteractionManager or the interactable will handle movement via MoveToWithInteraction if needed.
                    return;
                }
            }

            Vector2 feetPoint = clickPos;
            if (feetPosition != null)
            {
                feetPoint += (Vector2)feetPosition.localPosition;
            }

            if (walkArea != null && walkArea.OverlapPoint(feetPoint))
            {
                targetPosition = clickPos;
                isMoving = true;
                targetInteraction = null; // manual move, no interaction
            }
            else
            {
                Debug.Log("Clicked outside walkable area");
            }
        }
    }
    public void MoveToWithInteraction(Vector3 position, IInteractable interaction)
    {
        targetPosition = position;
        isMoving = true;
        targetInteraction = interaction;
    }
}