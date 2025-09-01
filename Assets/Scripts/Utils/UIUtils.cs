using UnityEngine;

public static class UIUtils
{
    /// <summary>
    /// Returns an active child GameObject at the given index under parent. If none exists, instantiates the prefab and returns it.
    /// Ensures the returned GameObject is active.
    /// </summary>
    public static GameObject GetOrCreateChild(Transform parent, GameObject prefab, int index)
    {
        if (parent == null) return null;

        if (index < parent.childCount)
        {
            var child = parent.GetChild(index).gameObject;
            if (!child.activeSelf) child.SetActive(true);
            return child;
        }

        if (prefab == null)
        {
            Debug.LogError("UIUtils.GetOrCreateChild: prefab is null and not enough children available.");
            return null;
        }

        var go = Object.Instantiate(prefab, parent);
        go.SetActive(true);
        return go;
    }

    /// <summary>
    /// Returns a component of type T from an existing child at index or instantiates the prefab and returns the component.
    /// </summary>
    public static T GetOrCreateChildComponent<T>(Transform parent, GameObject prefab, int index) where T : Component
    {
        var go = GetOrCreateChild(parent, prefab, index);
        if (go == null) return null;
        return go.GetComponent<T>();
    }
}
