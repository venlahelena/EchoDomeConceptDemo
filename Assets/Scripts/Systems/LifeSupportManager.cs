using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LifeSupportManager : MonoBehaviour
{
    [Header("Oxygen Settings")]
    [Range(0, 100)]
    public float oxygenLevel = 100f;
    public float oxygenDecreaseRate = 0.1f; // percent per second

    [Header("Power Settings")]
    [Range(0, 100)]
    public float powerLevel = 100f;
    public float powerDecreaseRate = 0.05f; // percent per second

    [Header("UI Elements")]
    public TextMeshProUGUI oxygenText;
    public TextMeshProUGUI powerText;

    private void Update()
    {
        // Power decreases over time
        powerLevel -= powerDecreaseRate * Time.deltaTime;
        powerLevel = Mathf.Clamp(powerLevel, 0f, 100f);

        // Oxygen decreases faster if power is low
        float currentOxygenDecreaseRate = oxygenDecreaseRate;
        if (powerLevel < 20f)
        {
            currentOxygenDecreaseRate *= 2f; // oxygen drains twice as fast
        }

        oxygenLevel -= currentOxygenDecreaseRate * Time.deltaTime;
        oxygenLevel = Mathf.Clamp(oxygenLevel, 0f, 100f);

        // Update UI
        if (oxygenText != null)
            oxygenText.text = $"Oxygen: {oxygenLevel:F1}%";

        if (powerText != null)
            powerText.text = $"Power: {powerLevel:F1}%";

        // Example warnings
        if (oxygenLevel <= 20f)
            Debug.LogWarning("Oxygen level critical!");

        if (powerLevel <= 20f)
            Debug.LogWarning("Power level critical!");
    }

    // Methods to adjust levels externally (for repairs, etc)
    public void IncreaseOxygen(float amount)
    {
        oxygenLevel = Mathf.Clamp(oxygenLevel + amount, 0f, 100f);
    }

    public void IncreasePower(float amount)
    {
        powerLevel = Mathf.Clamp(powerLevel + amount, 0f, 100f);
    }
}
