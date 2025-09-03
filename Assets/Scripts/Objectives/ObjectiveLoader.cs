using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scene helper that loads an ObjectiveCollectionSO into the ObjectiveManager on Start.
/// Designers can drop this on a scene root and assign a collection asset.
/// </summary>
[System.Obsolete("ObjectiveLoader is deprecated. Assign initial objectives on ObjectiveManager directly.")]
public class ObjectiveLoader : MonoBehaviour
{
    // kept for compatibility in older scenes but intentionally no-op to avoid duplicate loading
    void Start() { /* no-op */ }
}
