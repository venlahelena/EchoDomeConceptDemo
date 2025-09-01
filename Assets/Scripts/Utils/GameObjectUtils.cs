using UnityEngine;

public static class GameObjectUtils
{
    /// <summary>
    /// Ensures the root GameObject of the provided component is preserved across scene loads.
    /// </summary>
    public static void PreserveRoot(Component component)
    {
        if (component == null) return;
        var root = component.transform.root != null ? component.transform.root.gameObject : component.gameObject;
        Object.DontDestroyOnLoad(root);
    }
}
