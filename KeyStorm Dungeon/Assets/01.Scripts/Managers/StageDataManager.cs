using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDataManager : SingletonManager<StageDataManager>
{
    public StageSet easySet;
    public StageSet normalSet;
    public StageSet hardSet;

    Dictionary<int, StageData> stageMap = new Dictionary<int, StageData>();

    public StageDifficulty CurrentDifficulty { get; private set; }
    public int CurrentStageIndex { get; private set; }

    public StageSet CurrentStageSet { get; private set; }

    public StageData CurrentStageData { get; private set; }

    private void Start()
    {
        SelectDifficulty(StageDifficulty.Easy);
    }

    void SetStageMap()
    {
        CurrentStageSet = GetCurrentSet();

        foreach (var stage in CurrentStageSet.stageDatas)
        {
            if (stageMap.ContainsKey(stage.stageIndex))
            {
                continue;
            }
            stageMap.Add(stage.stageIndex, stage);
        }
    }

    public void SelectDifficulty(StageDifficulty difficulty)
    {
        CurrentDifficulty = difficulty;
        CurrentStageIndex = 1;
        SetStageMap();
        LoadCurrentStageData();
    }

    public void NextStage()
    {
        CurrentStageIndex++;
        LoadCurrentStageData();
    }

    void LoadCurrentStageData()
    {
        if (!stageMap.TryGetValue(CurrentStageIndex, out var data))
        {
            Debug.Log("모든 스테이지 클리어");
            return;
        }

        CurrentStageData = data;
    }

    StageSet GetCurrentSet()
    {
        return CurrentDifficulty switch
        {
            StageDifficulty.Easy => easySet,
            StageDifficulty.Normal => normalSet,
            StageDifficulty.Hard => hardSet,
            _ => easySet
        };
    }
}
