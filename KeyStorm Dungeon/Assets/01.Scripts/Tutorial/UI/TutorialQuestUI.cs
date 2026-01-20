using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialQuestUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private Transform objectiveContainer;
    [SerializeField] private GameObject objectivePrefab;

    [Header("색상")]
    [SerializeField] private Color incompleteColor = Color.white;
    [SerializeField] private Color completedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color checkmarkColor = new Color(0.2f, 0.8f, 0.2f);

    private List<TutorialObjectiveUI> activeObjectiveUIs = new List<TutorialObjectiveUI>();

    private void Awake()
    {
        questPanel?.SetActive(false);
    }

    public void ShowQuest(string title, List<QuestObjective> objectives)
    {
        if (questTitleText != null) questTitleText.text = title;

        ClearObjectiveUIs();

        foreach (var obj in objectives)
        {
            if (objectivePrefab == null || objectiveContainer == null) continue;

            GameObject go = Instantiate(objectivePrefab, objectiveContainer);
            TutorialObjectiveUI ui = go.GetComponent<TutorialObjectiveUI>();
            if (ui != null)
            {
                ui.Initialize(obj, incompleteColor, completedColor, checkmarkColor);
                activeObjectiveUIs.Add(ui);
            }
        }

        questPanel?.SetActive(true);
    }

    public void UpdateQuest(List<QuestObjective> objectives)
    {
        for (int i = 0; i < objectives.Count && i < activeObjectiveUIs.Count; i++)
        {
            activeObjectiveUIs[i].UpdateStatus(objectives[i]);
        }
    }

    public void HideQuest()
    {
        questPanel?.SetActive(false);
        ClearObjectiveUIs();
    }

    void ClearObjectiveUIs()
    {
        foreach (var ui in activeObjectiveUIs)
            if (ui != null) Destroy(ui.gameObject);
        activeObjectiveUIs.Clear();
    }
}