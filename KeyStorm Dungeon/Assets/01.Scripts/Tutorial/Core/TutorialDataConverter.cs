using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TutorialDataJson
{
    public List<TutorialStepJson> tutorialSteps;
}

[Serializable]
public class TutorialStepJson
{
    public int stepIndex;
    public string stepName;
    public string currentRoom;
    public List<DialogueLineJson> preDialogues;
    public bool hasQuest;
    public string questTitle;
    public List<QuestObjectiveJson> objectives;
    public List<DialogueLineJson> postDialogues;
    public bool resetMoveTrackingOnStart;
}

[Serializable]
public class DialogueLineJson
{
    public string text;
    public float typingSpeed;
    public bool waitForInput;
    public float autoDelay;
}

[Serializable]
public class QuestObjectiveJson
{
    public string description;
    public string type;
    public int targetCount;
}

public static class TutorialDataConverter
{
    public static List<TutorialStep> ConvertFromJson(string jsonContent)
    {
        TutorialDataJson data = JsonUtility.FromJson<TutorialDataJson>(jsonContent);
        List<TutorialStep> steps = new List<TutorialStep>();
        
        foreach (var stepJson in data.tutorialSteps)
        {
            TutorialStep step = ScriptableObject.CreateInstance<TutorialStep>();
            
            step.stepIndex = stepJson.stepIndex;
            step.stepName = stepJson.stepName;
            step.currentRoom = ParseRoomType(stepJson.currentRoom);
            step.hasQuest = stepJson.hasQuest;
            step.questTitle = stepJson.questTitle;
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
            
            steps.Add(step);
        }
        
        return steps;
    }
    
    private static TutorialRoomType ParseRoomType(string roomStr)
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
    
    private static QuestType ParseQuestType(string typeStr)
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
