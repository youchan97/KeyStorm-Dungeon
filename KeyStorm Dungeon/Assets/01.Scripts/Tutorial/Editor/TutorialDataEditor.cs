#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class TutorialDataEditor : EditorWindow
{
    private string jsonFilePath = "Assets/Data/TutorialDialogueData.json";
    private string outputFolder = "Assets/ScriptableObjects/Tutorial/Steps";
    
    [MenuItem("Tools/Tutorial/Create Steps from JSON")]
    public static void ShowWindow()
    {
        GetWindow<TutorialDataEditor>("Tutorial Data Editor");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Tutorial Data Converter", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        jsonFilePath = EditorGUILayout.TextField("JSON File Path", jsonFilePath);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Convert JSON to ScriptableObjects"))
        {
            ConvertJsonToScriptableObjects();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create Empty Step"))
        {
            CreateEmptyStep();
        }
    }
    
    private void ConvertJsonToScriptableObjects()
    {
        if (!File.Exists(jsonFilePath))
        {
            EditorUtility.DisplayDialog("Error", $"JSON file not found: {jsonFilePath}", "OK");
            return;
        }
        
        string jsonContent = File.ReadAllText(jsonFilePath);
        
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }
        
        TutorialDataJson data = JsonUtility.FromJson<TutorialDataJson>(jsonContent);
        
        if (data == null || data.tutorialSteps == null)
        {
            EditorUtility.DisplayDialog("Error", "Failed to parse JSON", "OK");
            return;
        }
        
        int createdCount = 0;
        
        foreach (var stepJson in data.tutorialSteps)
        {
            TutorialStep step = ScriptableObject.CreateInstance<TutorialStep>();
            
            step.stepIndex = stepJson.stepIndex;
            step.stepName = stepJson.stepName;
            step.currentRoom = ParseRoomType(stepJson.currentRoom);
            step.hasQuest = stepJson.hasQuest;
            step.questTitle = stepJson.questTitle ?? "";
            step.resetMoveTrackingOnStart = stepJson.resetMoveTrackingOnStart;
            step.blockInputDuringDialogue = true;
            
            step.preDialogues = new List<DialogueLine>();
            if (stepJson.preDialogues != null)
            {
                foreach (var dlg in stepJson.preDialogues)
                {
                    step.preDialogues.Add(new DialogueLine
                    {
                        text = dlg.text,
                        typingSpeed = dlg.typingSpeed > 0 ? dlg.typingSpeed : 0.03f,
                        waitForInput = dlg.waitForInput,
                        autoDelay = dlg.autoDelay > 0 ? dlg.autoDelay : 1.5f
                    });
                }
            }
            
            step.postDialogues = new List<DialogueLine>();
            if (stepJson.postDialogues != null)
            {
                foreach (var dlg in stepJson.postDialogues)
                {
                    step.postDialogues.Add(new DialogueLine
                    {
                        text = dlg.text,
                        typingSpeed = dlg.typingSpeed > 0 ? dlg.typingSpeed : 0.03f,
                        waitForInput = dlg.waitForInput,
                        autoDelay = dlg.autoDelay > 0 ? dlg.autoDelay : 1.5f
                    });
                }
            }
            
            step.objectives = new List<QuestObjective>();
            if (stepJson.objectives != null)
            {
                foreach (var obj in stepJson.objectives)
                {
                    step.objectives.Add(new QuestObjective
                    {
                        description = obj.description,
                        type = ParseQuestType(obj.type),
                        targetCount = obj.targetCount
                    });
                }
            }
            
            string fileName = $"Step_{stepJson.stepIndex:D2}_{SanitizeFileName(stepJson.stepName)}.asset";
            string assetPath = Path.Combine(outputFolder, fileName);
            
            AssetDatabase.CreateAsset(step, assetPath);
            createdCount++;
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("Success", $"Created {createdCount} TutorialStep assets", "OK");
    }
    
    private void CreateEmptyStep()
    {
        TutorialStep step = ScriptableObject.CreateInstance<TutorialStep>();
        step.stepName = "New Step";
        step.preDialogues = new List<DialogueLine>();
        step.postDialogues = new List<DialogueLine>();
        step.objectives = new List<QuestObjective>();
        
        string path = EditorUtility.SaveFilePanelInProject(
            "Save Tutorial Step",
            "NewTutorialStep",
            "asset",
            "Save the tutorial step"
        );
        
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(step, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = step;
        }
    }
    
    private string SanitizeFileName(string name)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            name = name.Replace(c.ToString(), "");
        }
        return name.Replace(" ", "_").Replace("-", "_");
    }
    
    private TutorialRoomType ParseRoomType(string roomStr)
    {
        return roomStr switch
        {
            "Start" => TutorialRoomType.Start,
            "Treasure" => TutorialRoomType.Treasure,
            "Normal" => TutorialRoomType.Normal,
            "Shop" => TutorialRoomType.Shop,
            "Boss" => TutorialRoomType.Boss,
            _ => TutorialRoomType.Start
        };
    }
    
    private QuestType ParseQuestType(string typeStr)
    {
        return typeStr switch
        {
            "MoveUp" => QuestType.MoveUp,
            "MoveDown" => QuestType.MoveDown,
            "MoveLeft" => QuestType.MoveLeft,
            "MoveRight" => QuestType.MoveRight,
            "Shoot" => QuestType.Shoot,
            "SpecialShoot" => QuestType.SpecialShoot,
            "EmptyMagazine" => QuestType.EmptyMagazine,
            "PickupItem" => QuestType.PickupItem,
            "BuyItem" => QuestType.BuyItem,
            "KillEnemy" => QuestType.KillEnemy,
            "KillBoss" => QuestType.KillBoss,
            "EnterRoom" => QuestType.EnterRoom,
            "EnterTreasureRoom" => QuestType.EnterTreasureRoom,
            "EnterNormalRoom" => QuestType.EnterNormalRoom,
            "EnterShopRoom" => QuestType.EnterShopRoom,
            "EnterBossRoom" => QuestType.EnterBossRoom,
            "UseBomb" => QuestType.UseBomb,
            "EnterPortal" => QuestType.EnterPortal,
            _ => QuestType.None
        };
    }
}
#endif
