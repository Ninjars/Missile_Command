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

    // Click handling
    private static readonly float CLICK_DISTANCE_THRESHOLD = 0.5f;
    private bool isClickDown = false;
    private Vector2 clickDownPosition = Vector2.zero;

    private void Awake() {
        var spawnedPlayerData = playerSpawner.performInitialSpawn();
        missileBatteries = spawnedPlayerData.missileBatteries;
        cities = spawnedPlayerData.cities;
        var bottomLeft = Camera.main.ViewportToWorldPoint(Vector2.zero);
        var topRight = Camera.main.ViewportToWorldPoint(Vector2.one);
        worldCoords = new WorldCoords(
            bottomLeft.x,
            topRight.x,
            bottomLeft.y, 
            topRight.y,
            0
        );
        Debug.Log($"worldCoords: {worldCoords.worldLeft}, {worldCoords.worldRight}, {worldCoords.centerX}");
        startNextLevel();
    }

    private void startNextLevel() {
        restockBases();

        LevelData levelData = levelManager.getLevelData();
        attackController.scheduleAttackEvents(
            worldCoords,
            cities,
            missileBatteries,
            levelData.icbmData,
            levelData.stageProgress
        );
    }

    internal void restockBases() {
        foreach (var battery in missileBatteries) {
            battery.setIsDestroyed(false);
            battery.missilesStored = battery.maxMissiles;
        }
    }

    private void onClick(float x, float y) {
        Debug.Log("onClick()");
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

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            isClickDown = true;
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            onClick(currentPosition.x, currentPosition.y);
        }
    }
}
