using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class PlantStateController : MonoBehaviour
{
    public enum PlantHealth { Thriving, Wilting, Sick, Dead }

    [Header("References")]
    [Tooltip("Optional: assign the LifeSupportManager in the inspector. If left empty, the script will try to find one at Start.")]
    public LifeSupportManager lifeSupportManager; // if null, will try to find one at Start

    [Header("Sprites by state")]
    public Sprite thrivingSprite;
    public Sprite wiltingSprite;
    public Sprite sickSprite;
    public Sprite deadSprite;

    [Header("Thresholds (percent)")]
    [Tooltip("Thriving when oxygen > thrivingThreshold")]
    public float thrivingThreshold = 70f;
    [Tooltip("Wilting between wiltingThreshold and thrivingThreshold")]
    public float wiltingThreshold = 50f;
    [Tooltip("Sick between sickThreshold and wiltingThreshold")]
    public float sickThreshold = 25f;

    [Header("Events")]
    public UnityEvent onThriving;
    public UnityEvent onWilting;
    public UnityEvent onSick;
    public UnityEvent onDead;

    [Header("Persistence")]
    [Tooltip("Optional unique ID for this plant used to persist its state between sessions.")]
    public string plantID;

    SpriteRenderer spriteRenderer;
    PlantHealth currentState = PlantHealth.Thriving;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (lifeSupportManager == null)
        {
            if (LifeSupportManager.Instance != null)
            {
                lifeSupportManager = LifeSupportManager.Instance;
            }
            else
            {
                Debug.LogWarning("PlantStateController: No LifeSupportManager instance found. Plant states will use default oxygen value.");
            }
        }

        // If a saved plant state exists, load it first
        if (!string.IsNullOrEmpty(plantID) && GameStateManager.Instance != null)
        {
            if (GameStateManager.Instance.TryGetPlantState(plantID, out int saved))
            {
                if (Enum.IsDefined(typeof(PlantHealth), saved))
                {
                    currentState = (PlantHealth)saved;
                    ApplySpriteForState(currentState);
                    InvokeEventForState(currentState);
                }
            }
            else
            {
                // No saved state; initialize based on oxygen
                UpdateState(true);
            }
        }
        else
        {
            // Initialize sprite based on current oxygen
            UpdateState(true);
        }
    }

    void Update()
    {
        UpdateState(false);
    }

    void UpdateState(bool forceEvent)
    {
        float oxygen = 100f;
        if (lifeSupportManager != null)
            oxygen = lifeSupportManager.oxygenLevel;

        PlantHealth newState;
        if (oxygen > thrivingThreshold)
            newState = PlantHealth.Thriving;
        else if (oxygen > wiltingThreshold)
            newState = PlantHealth.Wilting;
        else if (oxygen > sickThreshold)
            newState = PlantHealth.Sick;
        else
            newState = PlantHealth.Dead;

        if (newState != currentState || forceEvent)
        {
            currentState = newState;
            ApplySpriteForState(newState);
            InvokeEventForState(newState);
            // Persist the plant state if we have an ID
            if (!string.IsNullOrEmpty(plantID) && GameStateManager.Instance != null)
            {
                GameStateManager.Instance.SetPlantState(plantID, (int)currentState);
            }
        }
    }

    void ApplySpriteForState(PlantHealth state)
    {
        if (spriteRenderer == null) return;

        switch (state)
        {
            case PlantHealth.Thriving:
                if (thrivingSprite != null) spriteRenderer.sprite = thrivingSprite;
                break;
            case PlantHealth.Wilting:
                if (wiltingSprite != null) spriteRenderer.sprite = wiltingSprite;
                break;
            case PlantHealth.Sick:
                if (sickSprite != null) spriteRenderer.sprite = sickSprite;
                break;
            case PlantHealth.Dead:
                if (deadSprite != null) spriteRenderer.sprite = deadSprite;
                break;
        }
    }

    void InvokeEventForState(PlantHealth state)
    {
        switch (state)
        {
            case PlantHealth.Thriving:
                onThriving?.Invoke();
                break;
            case PlantHealth.Wilting:
                onWilting?.Invoke();
                break;
            case PlantHealth.Sick:
                onSick?.Invoke();
                break;
            case PlantHealth.Dead:
                onDead?.Invoke();
                break;
        }
    }

    // Public accessor
    public PlantHealth GetState() => currentState;
}
