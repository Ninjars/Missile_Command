using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerSpawner), typeof(ObjectPoolManager), typeof(LevelManager))]
[RequireComponent(typeof(AttackController), typeof(EvacuationController), typeof(UiController))]
public class GameController : MonoBehaviour {
    private UiController uiController;
    private PlayerSpawner playerSpawner;
    private ObjectPoolManager objectPoolManager;
    private LevelManager levelManager;
    private AttackController attackController;
    private EvacuationController evacuationController;
    [Tooltip("Percentage of the population that will be evacuated by the end of the game if no cities are destroyed")]
    public float evacuationFactor = 0.5f;

    public InputActionMap inGameInput;

    private WorldCoords worldCoords;
    private GameState gameState;

    // Click handling
    private static readonly float CLICK_DISTANCE_THRESHOLD = 0.5f;
    private bool isClickDown = false;
    private Vector2 clickDownPosition = Vector2.zero;

    private void Awake() {
        playerSpawner = GetComponent<PlayerSpawner>();
        objectPoolManager = GetComponent<ObjectPoolManager>();
        levelManager = GetComponent<LevelManager>();
        attackController = GetComponent<AttackController>();
        evacuationController = GetComponent<EvacuationController>();
        uiController = GetComponent<UiController>();

        var bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);
        var topRight = Camera.main.ViewportToWorldPoint(Vector2.one);
        worldCoords = new WorldCoords(
            bottomLeft.x,
            topRight.x,
            bottomLeft.y, 
            topRight.y,
            0
        );

        inGameInput["Fire 1"].performed += fireOne;
        inGameInput["Fire 2"].performed += fireTwo;
        inGameInput["Fire 3"].performed += fireThree;

        gameState = new GameState();
    }

    private void Update() {
        updateStateMachine();
    }

    private void updateStateMachine() {
        switch (gameState.currentMode) {
            case GameMode.MAIN_MENU: {
                inGameInput.Disable();
                uiController.setUiMode(UiMode.MAIN_MENU);
                break;
            }
            case GameMode.START_GAME: {
                uiController.setUiMode(UiMode.IN_GAME);
                gameState.onLevelPrepare();
                break;
            }
            case GameMode.PRE_LEVEL: {
                startNextLevel();
                inGameInput.Enable();
                gameState.onLevelBegin();
                break;
            }
            case GameMode.IN_LEVEL: {
                if (gameState.hasLost) {
                    gameState.onGameEnded(false);

                } else if (gameState.hasWon) {
                    gameState.onGameEnded(true);

                } else if (gameState.isLevelComplete) {
                    gameState.onLevelCompleted();
                }
                break;
            }
            case GameMode.POST_LEVEL: {
                levelManager.onLevelCompleted();
                evacuationController.performEvacuation();
                inGameInput.Disable();

                if (levelManager.allStagesCompleted) {
                    gameState.onGameEnded(true);
                } else {
                    gameState.onLevelPrepare();
                }
                break;
            }
            case GameMode.GAME_LOST: {
                inGameInput.Disable();
                attackController.stopAttacks();
                uiController.setUiMode(UiMode.LOSE_SCREEN);
                break;
            }
            case GameMode.GAME_WON: {
                inGameInput.Disable();
                attackController.stopAttacks();
                uiController.setUiMode(UiMode.WIN_SCREEN);
                break;
            }
        }
    }

    private void startNextLevel() {
        foreach (var battery in gameState.missileBatteries) {
            battery.restore();
        }

        LevelData levelData = levelManager.getLevelData();
        attackController.scheduleAttackEvents(
            gameState,
            worldCoords,
            gameState.cities,
            gameState.missileBatteries,
            levelData.icbmData,
            levelData.stageProgress
        );
    }

    private void fireOne(InputAction.CallbackContext context) {
        fire(gameState.missileBatteries[0]);
    }

    private void fireTwo(InputAction.CallbackContext context) {
        fire(gameState.missileBatteries[1]);
    }

    private void fireThree(InputAction.CallbackContext context) {
        fire(gameState.missileBatteries[2]);
    }

    private void fire(MissileBattery battery) {
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        battery.fire(targetPos.x, targetPos.y);
    }

    public void onUiStart() {
        startNewGame();
    }

    public void onUiMainMenu() {
        attackController.stopAttacks();
        clearBoard();
        gameState = new GameState();
    }

    public void onUiRestart() {
        attackController.stopAttacks();
        startNewGame();
    }

    private void startNewGame() {
        clearBoard();

        gameState = new GameState();

        var spawnedPlayerData = playerSpawner.performInitialSpawn(gameState, worldCoords, levelManager.getTotalLevels(), evacuationFactor);
        gameState.missileBatteries = spawnedPlayerData.missileBatteries;
        gameState.cities = spawnedPlayerData.cities;
        
        gameState.onGameBegin();
        evacuationController.initialise(gameState);
        uiController.setGameState(gameState);
    }

    private void clearBoard() {
        if (gameState.missileBatteries != null) {
            foreach (var battery in gameState.missileBatteries) {
                GameObject.Destroy(battery.gameObject);
            }
        }
        if (gameState.cities != null) {
            foreach (var city in gameState.cities) {
                GameObject.Destroy(city.gameObject);
            }
        }

        var allObjects = GameObject.FindObjectsOfType<GameObject>();
        clearAttacks(allObjects);
        clearExplosions(allObjects);
    }

    private void clearAttacks(GameObject[] objects) {
        foreach (var obj in objects) {
            if (obj.layer == LayerMask.NameToLayer("Enemy Munitions")) {
                obj.SetActive(false);
            }
        }
    }

    private void clearExplosions(GameObject[] objects) {
        foreach (var obj in objects) {
            if (obj.layer == LayerMask.NameToLayer("Explosions")) {
                obj.SetActive(false);
            }
        }
    }
}
