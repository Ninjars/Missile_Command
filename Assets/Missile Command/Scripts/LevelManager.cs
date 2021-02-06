using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public StagesList stageData;
    private List<StageData> stages { get { return stageData.stages; } }

    private int currentStage;
    private int stageLevel;

    private LevelData? currentLevelData;
    private bool _allStagesCompleted = false;
    public bool allStagesCompleted { get { return _allStagesCompleted; } }
    public bool beginningNewStage { get { return stageLevel == 0; } }

    public void reset() {
        currentStage = 0;
        stageLevel = 0;
        _allStagesCompleted = false;
        currentLevelData = null;
    }

    public void onLevelCompleted() {
        currentLevelData = null;
        if (currentStage >= stages.Count) {
            Debug.LogError($"unable to start stage {currentStage}; exceeds scheduled stages. Should have ended the game.");
            _allStagesCompleted = true;
            return;
        }
        stageLevel++;
        StageData currentStageData = stages[currentStage];
        if (stageLevel >= currentStageData.levels) {
            currentStage++;
            stageLevel = 0;
        }
        _allStagesCompleted = currentStage >= stages.Count;
    }

    public LevelData getLevelData() {
        if (currentLevelData.HasValue) {
            return currentLevelData.Value;
        } else {
            StageData currentData = stages[currentStage];
            var levelData = createLevelData(currentData, stageLevel);
            currentLevelData = levelData;
            return levelData;
        }
    }

    public int getTotalLevels() {
        return stages.Select(stage => stage.levels).Aggregate(0, (acc, next) => acc + next);
    }

    private LevelData createLevelData(StageData currentData, int stageLevel) {
        float stageProgress = stageLevel / (float)(currentData.levels - 1);
        var title = stageLevel == 0 ? currentData.title : null;
        return new LevelData(title, stageProgress, currentData.levelDuration.evaluate(stageProgress), currentData.weaponData);
    }
}

public struct LevelData {
    public readonly string title;
    public readonly float stageProgress;
    public readonly float stageDuration;
    public readonly List<WeaponData> weaponData;

    public LevelData(string title, float stageProgress, float stageDuration, List<WeaponData> weaponData) {
        this.title = title;
        this.stageProgress = stageProgress;
        this.stageDuration = stageDuration;
        this.weaponData = weaponData;
    }
}
