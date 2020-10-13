using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerSpawner), typeof(ObjectPoolManager), typeof(LevelManager))]
[RequireComponent(typeof(AttackController))]
public class GameController : MonoBehaviour {
    public PlayerSpawner playerSpawner;
    public ObjectPoolManager objectPoolManager;
    public LevelManager levelManager;
    public AttackController attackController;
    [Tooltip("Percentage of the population that will be evacuated by the end of the game if no cities are destroyed")]
    public float evacuationFactor = 0.5f;

    public InputActionMap inGameInput;

    private List<MissileBattery> missileBatteries;
    private List<City> cities;

    private WorldCoords worldCoords;
    private GameState gameState;

    // Click handling
    private static readonly float CLICK_DISTANCE_THRESHOLD = 0.5f;
    private bool isClickDown = false;
    private Vector2 clickDownPosition = Vector2.zero;

    private void Awake() {
        var bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);
        var topRight = Camera.main.ViewportToWorldPoint(Vector2.one);
        worldCoords = new WorldCoords(
            bottomLeft.x,
            topRight.x,
            bottomLeft.y, 
            topRight.y,
            0
        );

        var spawnedPlayerData = playerSpawner.performInitialSpawn(worldCoords, levelManager.getTotalLevels(), evacuationFactor);
        missileBatteries = spawnedPlayerData.missileBatteries;
        cities = spawnedPlayerData.cities;

        gameState = new GameState(cities);
        gameState.onLevelPrepare();

        inGameInput["Fire 1"].performed += fireOne;
        inGameInput["Fire 2"].performed += fireTwo;
        inGameInput["Fire 3"].performed += fireThree;
    }

    private void Update() {
        updateStateMachine();
    }

    private void updateStateMachine() {
        switch (gameState.currentMode) {
            case GameMode.PRE_LEVEL: {
                startNextLevel();
                inGameInput.Enable();
                gameState.onLevelBegin();
                break;
            }
            case GameMode.IN_LEVEL: {
                if (gameState.hasLost) {
                    gameState.onGameEnded(false);

                } else if (gameState.isLevelComplete) {
                    gameState.onLevelCompleted();
                }
                break;
            }
            case GameMode.POST_LEVEL: {
                levelManager.onLevelCompleted();
                inGameInput.Disable();

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

    private void fireOne(InputAction.CallbackContext context) {
        fire(missileBatteries[0]);
    }

    private void fireTwo(InputAction.CallbackContext context) {
        fire(missileBatteries[1]);
    }

    private void fireThree(InputAction.CallbackContext context) {
        fire(missileBatteries[2]);
    }

    private void fire(MissileBattery battery) {
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        battery.fire(targetPos.x, targetPos.y);
    }
}
