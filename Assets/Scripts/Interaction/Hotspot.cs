using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotspot : MonoBehaviour
{
    public Transform interactionPoint;
    public MonoBehaviour interactionScript;
    private SpriteRenderer iconRenderer;
    private Color defaultColor;
    public Color highlightColor = Color.cyan;

    void Start()
    {
        iconRenderer = GetComponentInChildren<SpriteRenderer>();
        if (iconRenderer != null)
        {
            defaultColor = iconRenderer.color;
        }
    }
    void OnMouseDown()
    {
        if (interactionScript is IInteractable interactable)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                PlayerMovement mover = player.GetComponent<PlayerMovement>();
                if (mover != null)
                {
                    mover.MoveToWithInteraction(interactionPoint.position, interactable);
                }
            }
        }
    }

    void OnMouseEnter()
    {
        if (iconRenderer != null)
        {
            iconRenderer.color = highlightColor;
        }
    }

    void OnMouseExit()
    {
        if (iconRenderer != null)
        {
            iconRenderer.color = defaultColor;
        }
    }
}