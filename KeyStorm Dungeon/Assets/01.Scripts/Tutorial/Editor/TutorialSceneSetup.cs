#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class TutorialSceneSetup : EditorWindow
{
    [MenuItem("Tools/Tutorial/Setup Tutorial Scene")]
    public static void ShowWindow()
    {
        GetWindow<TutorialSceneSetup>("Tutorial Scene Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tutorial Scene Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("1. Create Manager Objects"))
            CreateManagers();

        if (GUILayout.Button("2. Create Tutorial UI Canvas"))
            CreateUI();

        if (GUILayout.Button("3. Create Objective Prefab"))
            CreateObjectivePrefab();

        GUILayout.Space(20);
        if (GUILayout.Button("Create All"))
        {
            CreateManagers();
            CreateUI();
            CreateObjectivePrefab();
        }
    }

    void CreateManagers()
    {
        GameObject parent = new GameObject("TUTORIAL MANAGERS");

        GameObject tm = new GameObject("TutorialManager");
        tm.transform.SetParent(parent.transform);
        tm.AddComponent<TutorialManager>();

        GameObject ph = new GameObject("TutorialPlayerHook");
        ph.transform.SetParent(parent.transform);
        ph.AddComponent<TutorialPlayerHook>();
    }

    void CreateUI()
    {
        // Canvas
        GameObject canvasObj = new GameObject("TutorialCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();

        // Dialogue Panel (하단)
        CreateDialoguePanel(canvasObj.transform);

        // Quest Panel (우측 상단)
        CreateQuestPanel(canvasObj.transform);

        // Skip Button (좌측 상단)
        CreateSkipButton(canvasObj.transform);
    }

    void CreateDialoguePanel(Transform parent)
    {
        GameObject panel = new GameObject("DialoguePanel");
        panel.transform.SetParent(parent);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.pivot = new Vector2(0.5f, 0);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(0, 200);

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.8f);

        panel.AddComponent<TutorialDialogueUI>();

        // Text
        GameObject textObj = new GameObject("DialogueText");
        textObj.transform.SetParent(panel.transform);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(50, 30);
        textRect.offsetMax = new Vector2(-50, -30);

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 32;
        tmp.color = Color.white;

        // Continue Indicator
        GameObject contObj = new GameObject("ContinueIndicator");
        contObj.transform.SetParent(panel.transform);
        RectTransform contRect = contObj.AddComponent<RectTransform>();
        contRect.anchorMin = new Vector2(1, 0);
        contRect.anchorMax = new Vector2(1, 0);
        contRect.pivot = new Vector2(1, 0);
        contRect.anchoredPosition = new Vector2(-30, 20);
        contRect.sizeDelta = new Vector2(200, 30);

        TextMeshProUGUI contTmp = contObj.AddComponent<TextMeshProUGUI>();
        contTmp.fontSize = 20;
        contTmp.color = new Color(1, 1, 1, 0.7f);
        contTmp.alignment = TextAlignmentOptions.Right;
        contTmp.text = "클릭하여 계속 ";
    }

    void CreateQuestPanel(Transform parent)
    {
        GameObject panel = new GameObject("QuestPanel");
        panel.transform.SetParent(parent);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = new Vector2(-30, -30);
        rect.sizeDelta = new Vector2(350, 250);

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.7f);

        VerticalLayoutGroup vlg = panel.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(20, 20, 15, 15);
        vlg.spacing = 10;

        panel.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        panel.AddComponent<TutorialQuestUI>();

        // Title
        GameObject titleObj = new GameObject("QuestTitle");
        titleObj.transform.SetParent(panel.transform);
        titleObj.AddComponent<RectTransform>().sizeDelta = new Vector2(0, 40);
        TextMeshProUGUI titleTmp = titleObj.AddComponent<TextMeshProUGUI>();
        titleTmp.fontSize = 28;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.color = new Color(1, 0.85f, 0.3f);
        titleTmp.text = "퀘스트";

        // Container
        GameObject container = new GameObject("ObjectiveContainer");
        container.transform.SetParent(panel.transform);
        container.AddComponent<RectTransform>();
        VerticalLayoutGroup cvlg = container.AddComponent<VerticalLayoutGroup>();
        cvlg.spacing = 8;
        container.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    void CreateSkipButton(Transform parent)
    {
        GameObject btnObj = new GameObject("SkipButton");
        btnObj.transform.SetParent(parent);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(30, -30);
        rect.sizeDelta = new Vector2(100, 40);

        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        btnObj.AddComponent<Button>();
        btnObj.AddComponent<TutorialSkipUI>();

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 20;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.text = "스킵";

        // Confirm Panel
        CreateConfirmPanel(parent);
    }

    void CreateConfirmPanel(Transform parent)
    {
        GameObject panel = new GameObject("ConfirmPanel");
        panel.transform.SetParent(parent);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.7f);

        // Dialog Box
        GameObject box = new GameObject("DialogBox");
        box.transform.SetParent(panel.transform);
        RectTransform boxRect = box.AddComponent<RectTransform>();
        boxRect.anchorMin = new Vector2(0.5f, 0.5f);
        boxRect.anchorMax = new Vector2(0.5f, 0.5f);
        boxRect.sizeDelta = new Vector2(450, 200);

        Image boxImg = box.AddComponent<Image>();
        boxImg.color = new Color(0.15f, 0.15f, 0.15f);

        // Message
        GameObject msgObj = new GameObject("Message");
        msgObj.transform.SetParent(box.transform);
        RectTransform msgRect = msgObj.AddComponent<RectTransform>();
        msgRect.anchorMin = new Vector2(0, 0.4f);
        msgRect.anchorMax = new Vector2(1, 1);
        msgRect.offsetMin = new Vector2(20, 0);
        msgRect.offsetMax = new Vector2(-20, -20);

        TextMeshProUGUI msgTmp = msgObj.AddComponent<TextMeshProUGUI>();
        msgTmp.fontSize = 24;
        msgTmp.color = Color.white;
        msgTmp.alignment = TextAlignmentOptions.Center;
        msgTmp.text = "튜토리얼을 건너뛰시겠습니까?\n게임 방법을 모르면 어려울 수 있습니다.";

        // Buttons
        CreateConfirmButton(box.transform, "YesButton", "예", new Vector2(-80, -70));
        CreateConfirmButton(box.transform, "NoButton", "아니오", new Vector2(80, -70));

        panel.SetActive(false);
    }

    void CreateConfirmButton(Transform parent, string name, string text, Vector2 pos)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent);

        RectTransform rect = btnObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(120, 45);

        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.3f, 0.3f, 0.3f);
        btnObj.AddComponent<Button>();

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 22;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.text = text;
    }

    void CreateObjectivePrefab()
    {
        GameObject prefab = new GameObject("ObjectiveItem");
        prefab.AddComponent<RectTransform>().sizeDelta = new Vector2(0, 35);

        HorizontalLayoutGroup hlg = prefab.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 10;
        hlg.childControlWidth = false;
        hlg.childControlHeight = true;

        // Empty Box
        GameObject emptyBox = new GameObject("EmptyBox");
        emptyBox.transform.SetParent(prefab.transform);
        emptyBox.AddComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
        emptyBox.AddComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);

        // Checkmark
        GameObject checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(prefab.transform);
        checkmark.AddComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
        checkmark.AddComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
        checkmark.SetActive(false);

        // Description
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(prefab.transform);
        descObj.AddComponent<RectTransform>().sizeDelta = new Vector2(250, 30);
        TextMeshProUGUI descTmp = descObj.AddComponent<TextMeshProUGUI>();
        descTmp.fontSize = 22;
        descTmp.color = Color.white;

        // Progress
        GameObject progObj = new GameObject("Progress");
        progObj.transform.SetParent(prefab.transform);
        progObj.AddComponent<RectTransform>().sizeDelta = new Vector2(50, 30);
        TextMeshProUGUI progTmp = progObj.AddComponent<TextMeshProUGUI>();
        progTmp.fontSize = 20;
        progTmp.color = new Color(0.7f, 0.7f, 0.7f);

        prefab.AddComponent<TutorialObjectiveUI>();

        // Save Prefab
        string path = "Assets/Prefabs/Tutorial";
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        PrefabUtility.SaveAsPrefabAsset(prefab, path + "/ObjectiveItem.prefab");
        DestroyImmediate(prefab);

        Debug.Log("ObjectiveItem prefab created!");
    }
}
#endif