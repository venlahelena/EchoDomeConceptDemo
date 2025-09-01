using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveItem : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public GameObject completedIcon; // optional - shown when completed

    public void SetData(ObjectiveManager.Objective obj)
    {
        if (titleText != null) titleText.text = obj.title ?? "(untitled)";
        if (descriptionText != null) descriptionText.text = obj.description ?? "";
        if (completedIcon != null) completedIcon.SetActive(obj.completed);
    }
}
