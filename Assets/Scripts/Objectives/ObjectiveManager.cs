using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;

    public TMPro.TextMeshProUGUI objectiveText;

    private string currentObjective;

    void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }
    Instance = this;
    DontDestroyOnLoad(transform.root.gameObject);
}


    public void SetObjective(string newObjective)
    {
        currentObjective = newObjective;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (objectiveText != null)
            objectiveText.text = "Objective: " + currentObjective;
    }
}

