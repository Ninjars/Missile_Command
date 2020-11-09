﻿using System.Collections;
using System.Collections.Generic;
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
    public long baseEvacuees = 100;
    public long initialCityPopulation = 1000000;

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
        backdropGenerator = GetComponent<BackdropGenerator>();

        Camera.main.backgroundColor = Colors.Instance.skyColor.from;

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

        gameState = new GameState(baseEvacRate, baseEvacuees);
    }

    private void Update() {
        updateStateMachine();
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
                gameState.onLevelPrepare();
                showAllCityUi();
                break;
            }
            case GameMode.PRE_LEVEL: {
                startNextLevel();
                inGameInput.Enable();
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
            case GameMode.POST_LEVEL: {
                inGameInput.Disable();
                levelManager.onLevelCompleted();
                evacuationController.suspendEvacuations();

                if (levelManager.allStagesCompleted) {
                    gameState.onGameEnded(true);
                    
                } else if (levelManager.beginningNewStage) {
                    clearEvacuators();
                    showAllCityUi();
                    gameState.onLevelPrepare();

                } else {
                    gameState.onLevelPrepare();
                }
                break;
            }
            case GameMode.GAME_LOST: {
                inGameInput.Disable();
                attackController.stopAttacks();
                evacuationController.suspendEvacuations();
                clearEvacuators();
                showAllCityUi();
                uiController.setUiMode(UiMode.LOSE_SCREEN);
                break;
            }
            case GameMode.GAME_WON: {
                inGameInput.Disable();
                attackController.stopAttacks();
                evacuationController.suspendEvacuations();
                clearEvacuators();
                showAllCityUi();
                uiController.setUiMode(UiMode.WIN_SCREEN);
                break;
            }
        }
    }

    private bool hasLevelEnded() {
        return gameState.canEndLevel(Time.time) && GameObject.FindGameObjectsWithTag("Attack").Length == 0;
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
        gameState = new GameState(baseEvacRate, baseEvacuees);
    }

    public void onUiRestart() {
        attackController.stopAttacks();
        startNewGame();
    }

    private void startNewGame() {
        clearBoard();
        backdropGenerator.generateBackground(worldCoords);

        gameState = new GameState(baseEvacRate, baseEvacuees);

        var spawnedPlayerData = playerSpawner.performInitialSpawn(gameState, worldCoords, levelManager.getTotalLevels(), initialCityPopulation);
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

    private void clearEvacuators() {
        foreach (var evacuatorObj in GameObject.FindGameObjectsWithTag("Evacuator")) {
            Evacuator evacuator = evacuatorObj.GetComponent<Evacuator>();
            evacuator.deliver();
        }
    }
}
