using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveUIController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelRoot; // whole objective panel (collapsible)
    public RectTransform contentRoot; // where objective items are instantiated
    public GameObject objectiveItemPrefab; // prefab with ObjectiveItem script
    public TextMeshProUGUI headerText; // collapsed short text
    public Image arrowImage; // rotate to indicate expanded/collapsed
    public float arrowRotateDuration = 0.18f;

    bool expanded = false;

    void Start()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnObjectiveAdded += OnObjectiveChanged;
            ObjectiveManager.Instance.OnObjectiveCompleted += OnObjectiveChanged;
        }

        RefreshUI();
        SetExpanded(false, true);
    }

    void OnDestroy()
    {
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnObjectiveAdded -= OnObjectiveChanged;
            ObjectiveManager.Instance.OnObjectiveCompleted -= OnObjectiveChanged;
        }
    }

    public void Toggle()
    {
        SetExpanded(!expanded);
    }

    public void SetExpanded(bool value, bool instant = false)
    {
        expanded = value;
        if (contentRoot != null)
            contentRoot.gameObject.SetActive(expanded);

        if (arrowImage != null)
        {
            StopAllCoroutines();
            float target = expanded ? 180f : 0f;
            if (instant)
                arrowImage.rectTransform.localEulerAngles = new Vector3(0, 0, target);
            else
                StartCoroutine(RotateArrow(arrowImage.rectTransform, target));
        }

        RefreshUI();
    }

    IEnumerator RotateArrow(RectTransform rt, float targetZ)
    {
        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, arrowRotateDuration);
        float startZ = rt.localEulerAngles.z;
        if (startZ > 180f) startZ -= 360f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float z = Mathf.Lerp(startZ, targetZ, t);
            rt.localEulerAngles = new Vector3(0, 0, z);
            yield return null;
        }
        rt.localEulerAngles = new Vector3(0, 0, targetZ);
    }

    void OnObjectiveChanged(ObjectiveManager.Objective obj)
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (ObjectiveManager.Instance == null) return;

        var list = ObjectiveManager.Instance.GetAllObjectives();

        // Update header collapsed text
        if (headerText != null)
        {
            var active = ObjectiveManager.Instance.CurrentObjective;
            if (active != null)
                headerText.text = active.title;
            else
                headerText.text = "No Objectives";
        }

        if (!expanded) return;

        if (contentRoot == null || objectiveItemPrefab == null) return;

        // Reuse existing children where possible, instantiate extra if needed
        int i = 0;
        for (; i < list.Count; i++)
        {
            var item = UIUtils.GetOrCreateChildComponent<ObjectiveItem>(contentRoot, objectiveItemPrefab, i);
            if (item == null) continue;
            item.SetData(list[i]);
        }

        // Deactivate any leftover UI children
        for (; i < contentRoot.childCount; i++)
        {
            contentRoot.GetChild(i).gameObject.SetActive(false);
        }
    }
}
