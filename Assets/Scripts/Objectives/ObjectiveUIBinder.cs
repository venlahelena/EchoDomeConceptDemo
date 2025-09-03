using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Helper that binds a TextMeshProUGUI (usually the ObjectiveUIController headerText)
/// into ObjectiveManager.objectiveText so the top-line objective displays automatically.
/// Place this on the same GameObject as `ObjectiveUIController` or any scene root.
/// </summary>
public class ObjectiveUIBinder : MonoBehaviour
{
    [Tooltip("Optional: assign the TMP header text to bind. If empty, will try to find ObjectiveUIController.headerText on the same GameObject.")]
    public TextMeshProUGUI headerText;

    IEnumerator Start()
    {
        if (headerText == null)
        {
            var uiController = GetComponent<ObjectiveUIController>();
            if (uiController != null)
                headerText = uiController.headerText;
        }
        // Wait up to 0.5s for ObjectiveManager to become available
        float timeout = 0.5f; float elapsed = 0f;
        while (ObjectiveManager.Instance == null && elapsed < timeout)
        {
            elapsed += Time.deltaTime; yield return null;
        }

        if (ObjectiveManager.Instance != null && headerText != null)
            ObjectiveManager.Instance.objectiveText = headerText;
    }
}
