using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialObjectiveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private GameObject checkmark;
    [SerializeField] private GameObject emptyBox;

    private Color incompleteColor, completedColor;

    public void Initialize(QuestObjective obj, Color incomplete, Color completed, Color checkColor)
    {
        incompleteColor = incomplete;
        completedColor = completed;

        if (descriptionText != null)
        {
            descriptionText.text = obj.description;
            descriptionText.color = incompleteColor;
        }

        UpdateProgressText(obj);
        UpdateCheckmark(obj.IsCompleted);

        if (checkmark != null)
        {
            Image img = checkmark.GetComponent<Image>();
            if (img != null) img.color = checkColor;
        }
    }

    public void UpdateStatus(QuestObjective obj)
    {
        UpdateProgressText(obj);
        UpdateCheckmark(obj.IsCompleted);

        if (descriptionText != null)
            descriptionText.color = obj.IsCompleted ? completedColor : incompleteColor;
    }

    void UpdateProgressText(QuestObjective obj)
    {
        if (progressText == null) return;

        if (obj.targetCount > 1)
        {
            progressText.gameObject.SetActive(true);
            progressText.text = $"{obj.currentCount}/{obj.targetCount}";
        }
        else
        {
            progressText.gameObject.SetActive(false);
        }
    }

    void UpdateCheckmark(bool completed)
    {
        checkmark?.SetActive(completed);
        emptyBox?.SetActive(!completed);
    }
}