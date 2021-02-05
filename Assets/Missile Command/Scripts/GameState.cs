using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameState : StateUpdater {
    public int levelsCompleted;
    public List<City> cities;
    public List<MissileBattery> missileBatteries;
    public GameMode currentMode { get; private set; }
    public bool hasLost { 
        get {
            if (currentMode == GameMode.MAIN_MENU) return false;

            foreach (var city in cities) {
                if (!city.isDestroyed) {
                    return false;
                }
            } 
            return true; 
        }
    }
    public bool hasWon { 
        get {
            if (currentMode == GameMode.MAIN_MENU) return false;
            if (hasLost) return false;
            foreach (var city in cities) {
                if (city.population != 0) {
                    return false;
                }
            } 
            return true; 
        }
    }
    public long populationEvacuated { get; private set; }
    public long citiesPopulation { get { return cities.Aggregate(0L, (acc, city) => acc + city.population); } }
    public long populationDead { get; private set; }
    public float evacEventInterval { get; private set; }
    public int upgradePoints { get; private set; }

    public GameState(float evacEventsPerMin) {
        currentMode = GameMode.MAIN_MENU;
        this.evacEventInterval = evacEventsPerMin / 60f;
    }

    public void onPopulationEvacuated(long count) {
        var oldPopThreshold = Mathf.FloorToInt(populationEvacuated / 3000);
        populationEvacuated += count;
        
        if (Mathf.FloorToInt(populationEvacuated / 3000) != oldPopThreshold) {
            Debug.Log("evacuation upgrade point threshold reached");
            awardUpgradePoints(1);
        };
    }

    public void onPopulationLost(long count) {
        populationDead += count;
    }

    public void onUpgradePointSpent() {
        upgradePoints--;
    }

    public void awardUpgradePoints(int count) {
        upgradePoints += count;
    }

    internal bool canUpgradeSomething() {
        return upgradePoints > 0 
            && (
                cities.Where(city => city.upgradeState.hasAnyAvailableUpgrades).FirstOrDefault() != null
                || missileBatteries.Where(battery => battery.upgradeState.hasAnyAvailableUpgrades).FirstOrDefault() != null
            );
    }

    public void onGameBegin() {
        currentMode = GameMode.START_GAME;
    }

    public void onGameContinue() {
        Debug.Log("GameState.onGameContinue()");
        currentMode = GameMode.UPGRADING;
    }

    public void onLevelBegin() {
        Debug.Log("GameState.onLevelBegin()");
        currentMode = GameMode.IN_LEVEL;
    }

    public void onUpgradeCompleted() {
        Debug.Log("GameState.onUpgradeCompleted()");
        currentMode = GameMode.PRE_LEVEL;
    }

    public void onLevelCompleted() {
        Debug.Log("GameState.onLevelCompleted()");
        currentMode = GameMode.END_LEVEL;
    }

    public void onLevelEnding() {
        Debug.Log("GameState.onLevelEnding()");
        currentMode = GameMode.LEVEL_ENDING;
    }

    public void onLevelEnded() {
        Debug.Log("GameState.onLevelEnded()");
        currentMode = GameMode.POST_LEVEL;
        levelsCompleted++;
    }

    public void onGameEnded(bool gameWon) {
        Debug.Log($"GameState.onGameEnded(win: {gameWon})");
        currentMode = gameWon ? GameMode.GAME_WON : GameMode.GAME_LOST;
    }
}

public enum GameMode {
    MAIN_MENU,
    START_GAME,
    PRE_LEVEL,
    IN_LEVEL,
    END_LEVEL,
    LEVEL_ENDING,
    UPGRADING,
    POST_LEVEL,
    GAME_WON,
    GAME_LOST,
}

public interface StateUpdater {
    void onPopulationLost(long count);
}
