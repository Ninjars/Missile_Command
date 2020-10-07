using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerSpawner), typeof(ObjectPoolManager), typeof(LevelManager))]
[RequireComponent(typeof(AttackController))]
public class GameController : MonoBehaviour {
    public PlayerSpawner playerSpawner;
    public ObjectPoolManager objectPoolManager;
    public LevelManager levelManager;
    public AttackController attackController;

    private List<MissileBattery> missileBatteries;
    private List<City> cities;

    public int currentBatteryIndex = 0;
    private WorldCoords worldCoords;
    private GameState gameState;

    // Click handling
    private static readonly float CLICK_DISTANCE_THRESHOLD = 0.5f;
    private bool isClickDown = false;
    private Vector2 clickDownPosition = Vector2.zero;

    private void Awake() {
        var spawnedPlayerData = playerSpawner.performInitialSpawn();
        missileBatteries = spawnedPlayerData.missileBatteries;
        cities = spawnedPlayerData.cities;

        gameState = new GameState(cities);
        gameState.onLevelPrepare();

        var bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);
        var topRight = Camera.main.ViewportToWorldPoint(Vector2.one);
        worldCoords = new WorldCoords(
            bottomLeft.x,
            topRight.x,
            bottomLeft.y, 
            topRight.y,
            0
        );
    }

    private void Update() {
        updateStateMachine();
    }

    private void updateStateMachine() {
        switch (gameState.currentMode) {
            case GameMode.PRE_LEVEL: {
                startNextLevel();
                gameState.onLevelBegin();
                break;
            }
            case GameMode.IN_LEVEL: {
                checkGameInput();

                if (gameState.hasLost) {
                    gameState.onGameEnded(false);

                } else if (gameState.isLevelComplete) {
                    gameState.onLevelCompleted();
                }
                break;
            }
            case GameMode.POST_LEVEL: {
                levelManager.onLevelCompleted();

                if (levelManager.allStagesCompleted) {
                    gameState.onGameEnded(true);
                } else {
                    gameState.onLevelPrepare();
                }
                break;
            }
        }
    }

    private void startNextLevel() {
        foreach (var battery in missileBatteries) {
            battery.restore();
        }

        LevelData levelData = levelManager.getLevelData();
        attackController.scheduleAttackEvents(
            gameState,
            worldCoords,
            cities,
            missileBatteries,
            levelData.icbmData,
            levelData.stageProgress
        );
    }

    private void checkGameInput() {
        if (Input.GetButtonDown("Fire1")) {
            isClickDown = true;
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetMissile(currentPosition.x, currentPosition.y);
        }
    }

    private void targetMissile(float x, float y) {
        for (int i = 0; i < missileBatteries.Count; i++) {
            var index = (i + currentBatteryIndex) % (missileBatteries.Count);
            Debug.Log($"sending fire request to battery {index}");
            if (missileBatteries[index].fire(x, y)) {
                currentBatteryIndex = index + 1;
                break;
            }
            currentBatteryIndex++;
        }
    }
}
