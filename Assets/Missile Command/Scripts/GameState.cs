﻿using System.Collections;
using System.Collections.Generic;
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
    public long populationDead { get; private set; }
    public float evacEventInterval { get; private set; }
    public long evacEventCount { get; private set; }
    private float levelEndMin;

    public GameState(float evacEventsPerMin, long evacEventCount) {
        currentMode = GameMode.MAIN_MENU;
        this.evacEventInterval = evacEventsPerMin * 60f;
        this.evacEventCount = evacEventCount;
    }

    public void onPopulationEvacuated(long count) {
        populationEvacuated += count;
    }

    public void onPopulationLost(long count) {
        populationDead += count;
    }

    public void setLevelEnd(float timeStamp) {
        levelEndMin = timeStamp;
    }

    public bool canEndLevel(float currentTime) {
        return currentTime > levelEndMin;
    }

    public void onGameBegin() {
        currentMode = GameMode.START_GAME;
    }

    public void onLevelPrepare() {
        Debug.Log("GameState.onLevelPrepare()");
        currentMode = GameMode.PRE_LEVEL;
    }

    public void onLevelBegin() {
        Debug.Log("GameState.onLevelBegin()");
        currentMode = GameMode.IN_LEVEL;
    }

    public void onLevelCompleted() {
        Debug.Log("GameState.onLevelCompleted()");
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
    POST_LEVEL,
    GAME_WON,
    GAME_LOST,
}

public interface StateUpdater {
    void setLevelEnd(float timeStamp);
    void onPopulationLost(long count);
}
