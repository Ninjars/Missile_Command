using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour {
    public WaveProgressionData levelData;
    public int totalLevels { get; private set; }

    private List<StageDefinition> stages { get { return levelData.stages; } }

    private void Awake() {
        totalLevels = stages.Select(stage => stage.waveCount).Aggregate(0, (acc, next) => acc + next);
    }

    public LevelData getLevelData(int levelsCompleted) {
        int runningTotal = 0;
        foreach (var stage in stages) {
            runningTotal += stage.waveCount;
            if (runningTotal > levelsCompleted) {
                return createLevelData(stage, levelsCompleted);
            }
        }
        throw new System.Exception($"no stage found for level {levelsCompleted} of {totalLevels}");
    }

    private LevelData createLevelData(StageDefinition stage, int levelsCompleted) {
        return new LevelData(stage.title, 15, createWeaponSnapshots(stage, levelsCompleted / (float) totalLevels));
    }

    private List<WeaponSnapshot> createWeaponSnapshots(StageDefinition stage, float gameProgress) {
        List<WeaponSnapshot> snapshots = new List<WeaponSnapshot>();
        if (stage.icbm) {
            snapshots.Add(levelData.ICBM.createSnapshot(gameProgress));
        }
        if (stage.heavyMirv) {
            snapshots.Add(levelData.heavyMIRV.createSnapshot(gameProgress));
        }
        if (stage.rfg) {
            snapshots.Add(levelData.RFG.createSnapshot(gameProgress));
        }
        if (stage.standardBomber) {
            snapshots.Add(levelData.standardBomber.createSnapshot(gameProgress));
        }
        if (stage.strategicBomber) {
            snapshots.Add(levelData.strategicBomber.createSnapshot(gameProgress));
        }
        if (stage.hammer) {
            snapshots.Add(levelData.hammer.createSnapshot(gameProgress));
        }
        return snapshots;
    }
}

public struct LevelData {
    public readonly string title;
    public readonly float stageDuration;
    public readonly List<WeaponSnapshot> weaponData;

    public LevelData(string title, float stageDuration, List<WeaponSnapshot> weaponData) {
        this.title = title;
        this.stageDuration = stageDuration;
        this.weaponData = weaponData;
    }
}
