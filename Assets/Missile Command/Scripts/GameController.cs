using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerSpawner), typeof(ObjectPoolManager), typeof(LevelManager))]
[RequireComponent(typeof(AttackController), typeof(EvacuationController), typeof(UiController))]
[RequireComponent(typeof(BackdropGenerator))]
public class GameController : MonoBehaviour {
    private UiController uiController;
    private PlayerSpawner playerSpawner;
    private ObjectPoolManager objectPoolManager;
    private LevelManager levelManager;
    private AttackController attackController;
    private EvacuationController evacuationController;
    private BackdropGenerator backdropGenerator;
    [Tooltip("Initial evacuation events per minute")]
    public float baseEvacRate = 1f;
    [Tooltip("Initial evacuees per event")]
    public long basePopPerEvacEvent = 100;
    public int baseEvacEventsPerLevel = 3;
    public long initialCityPopulation = 1000000;

    public InputActionMap inGameInput;
    public CursorLines cursorLines;
    public Line baseLine;

    private WorldCoords worldCoords;
    private GameState gameState;

    // Click handling
    private static readonly float CLICK_DISTANCE_THRESHOLD = 0.5f;
    private bool isClickDown = false;
    private Vector2 clickDownPosition = Vector2.zero;

    private void Awake() {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
        playerSpawner = GetComponent<PlayerSpawner>();
        objectPoolManager = GetComponent<ObjectPoolManager>();
        levelManager = GetComponent<LevelManager>();
        attackController = GetComponent<AttackController>();
        evacuationController = GetComponent<EvacuationController>();
        uiController = GetComponent<UiController>();
        backdropGenerator = GetComponent<BackdropGenerator>();
        cursorLines.gameObject.SetActive(false);

        Camera.main.backgroundColor = Colors.Instance.skyColor.from;
        baseLine.Color = Colors.Instance.groundLineColor;

        inGameInput["Fire 1"].performed += fireOne;
        inGameInput["Fire 2"].performed += fireTwo;
        inGameInput["Fire 3"].performed += fireThree;

        gameState = new GameState(baseEvacRate);
    }

    private void Start() {
        var bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);
        var topRight = Camera.main.ViewportToWorldPoint(Vector2.one);
        worldCoords = new WorldCoords(
            bottomLeft.x,
            topRight.x,
            bottomLeft.y,
            topRight.y,
            0
        );
        baseLine.Start = new Vector2(worldCoords.worldLeft, 0);
        baseLine.End = new Vector2(worldCoords.worldRight, 0);
    }

    private void Update() {
        updateStateMachine();
    }

    private void showAllMissileBatteryLabels() {
        if (gameState.missileBatteries == null) return;
        foreach (var battery in gameState.missileBatteries) {
            battery.setLabelVisible(true);
        }
    }

    private void hideAllMissileBatteryLabels() {
        if (gameState.missileBatteries == null) return;
        foreach (var battery in gameState.missileBatteries) {
            battery.setLabelVisible(false);
        }
    }

    private void showAllCityUi() {
        if (gameState.cities == null) return;
        foreach (var city in gameState.cities) {
            city.showUi();
        }
    }

    private void hideAllCityUi() {
        if (gameState.cities == null) return;
        foreach (var city in gameState.cities) {
            city.fadeOutUi();
        }
    }

    private void clearAllCityUi() {
        if (gameState.cities == null) return;
        foreach (var city in gameState.cities) {
            city.hideUi();
        }
    }

    private void setCursorLinesActive(bool active) {
        cursorLines.gameObject.SetActive(active);
    }

    private void updateStateMachine() {
        switch (gameState.currentMode) {
            case GameMode.MAIN_MENU: {
                    inGameInput.Disable();
                    uiController.setUiMode(UiMode.MAIN_MENU);
                    clearAllCityUi();
                    break;
                }
            case GameMode.START_GAME: {
                    uiController.setUiMode(UiMode.IN_GAME);
                    levelManager.reset();
                    gameState.onLevelPrepare();
                    cursorLines.setBatteries(gameState.missileBatteries);
                    showAllCityUi();
                    break;
                }
            case GameMode.PRE_LEVEL: {
                    startNextLevel();
                    inGameInput.Enable();
                    setCursorLinesActive(true);
                    showAllMissileBatteryLabels();
                    evacuationController.beginEvacuations();
                    gameState.onLevelBegin();
                    hideAllCityUi();
                    break;
                }
            case GameMode.IN_LEVEL: {
                    if (gameState.hasLost) {
                        gameState.onGameEnded(false);

                    } else if (gameState.hasWon) {
                        gameState.onGameEnded(true);

                    } else if (hasLevelEnded()) {
                        gameState.onLevelCompleted();
                    }
                    break;
                }
            case GameMode.END_LEVEL: {
                    inGameInput.Disable();
                    setCursorLinesActive(false);
                    boostEvacuators();
                    evacuationController.completeEvacuations();
                    showAllCityUi();
                    uiController.setUiMode(UiMode.LEVEL_END);
                    gameState.onLevelEnding();
                    break;
                }
            case GameMode.LEVEL_ENDING: {
                    if (GameObject.FindGameObjectWithTag("Evacuator") == null && !uiController.isChoosingUpgrade) {
                        gameState.onLevelEnded();
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
            case GameMode.GAME_LOST: {
                    inGameInput.Disable();
                    attackController.stopAttacks();
                    evacuationController.clear();
                    setCursorLinesActive(false);
                    hideAllMissileBatteryLabels();
                    clearEvacuators();
                    showAllCityUi();
                    uiController.setUiMode(UiMode.LOSE_SCREEN);
                    break;
                }
            case GameMode.GAME_WON: {
                    inGameInput.Disable();
                    attackController.stopAttacks();
                    evacuationController.clear();
                    setCursorLinesActive(false);
                    hideAllMissileBatteryLabels();
                    clearEvacuators();
                    showAllCityUi();
                    uiController.setUiMode(UiMode.WIN_SCREEN);
                    break;
                }
        }
    }

    private bool hasLevelEnded() {
        return attackController.allAttacksLaunched() && GameObject.FindGameObjectsWithTag("Attack").Length == 0;
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
            levelData.weaponData,
            levelData.stageDuration,
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
        gameState = new GameState(baseEvacRate);
    }

    public void onUiRestart() {
        attackController.stopAttacks();
        startNewGame();
    }

    private void startNewGame() {
        clearBoard();
        backdropGenerator.generateBackground(worldCoords);

        gameState = new GameState(baseEvacRate);

        var spawnedPlayerData = playerSpawner.performInitialSpawn(
            gameState,
            worldCoords,
            levelManager.getTotalLevels(),
            initialCityPopulation,
            baseEvacEventsPerLevel,
            basePopPerEvacEvent
        );
        gameState.missileBatteries = spawnedPlayerData.missileBatteries;
        gameState.cities = spawnedPlayerData.cities;

        gameState.onGameBegin();
        evacuationController.initialise(gameState, worldCoords);
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
        clearEvacuators();
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

    private void boostEvacuators() {
        foreach (var evacuatorObj in GameObject.FindGameObjectsWithTag("Evacuator")) {
            Evacuator evacuator = evacuatorObj.GetComponent<Evacuator>();
            evacuator.boost();
        }
    }

    private void clearEvacuators() {
        foreach (var evacuatorObj in GameObject.FindGameObjectsWithTag("Evacuator")) {
            Evacuator evacuator = evacuatorObj.GetComponent<Evacuator>();
            evacuator.deliver();
        }
    }
}
