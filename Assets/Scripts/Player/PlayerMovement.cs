using UnityEngine;

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