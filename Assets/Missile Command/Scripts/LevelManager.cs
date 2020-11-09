using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public List<StageData> stageData;

    public int currentStage;
    public int stageLevel;

    private LevelData? currentLevelData;
    private bool _allStagesCompleted = false;
    public bool allStagesCompleted { get { return _allStagesCompleted; } }
    public bool beginningNewStage { get { return stageLevel == 0; } }

    public void onLevelCompleted() {
        currentLevelData = null;
        stageLevel++;
        StageData currentStageData = stageData[currentStage];
        if (stageLevel >= currentStageData.levels) {
            currentStage++;
            stageLevel = 0;
        }
        _allStagesCompleted = currentStage >= stageData.Count;
    }

    public LevelData getLevelData() {
        if (currentLevelData.HasValue) {
            return currentLevelData.Value;
        } else {
            StageData currentData = stageData[currentStage];
            var levelData = createLevelData(currentData, stageLevel / (float) (currentData.levels - 1));
            currentLevelData = levelData;
            return levelData;
        }
    }

    public int getTotalLevels() {
        return stageData.Select(stage => stage.levels).Aggregate(0, (acc, next) => acc + next);
    }

    private LevelData createLevelData(StageData currentData, float stageProgress) {
        var message = stageProgress == 0 ? currentData.title : null;
        return new LevelData(message, stageProgress, currentData.weaponData);
    }
}

public struct LevelData {
    public readonly string message;
    public readonly float stageProgress;
    public readonly List<WeaponData> weaponData;

    public LevelData(string message, float stageProgress, List<WeaponData> weaponData) {
        this.message = message;
        this.stageProgress = stageProgress;
        this.weaponData = weaponData;
    }
}
