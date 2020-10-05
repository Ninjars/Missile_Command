using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerSpawner), typeof(ObjectPoolManager))]
public class GameController : MonoBehaviour {
    public PlayerSpawner playerSpawner;
    public ObjectPoolManager objectPoolManager;

    private List<MissileBattery> missileBatteries;
    private List<City> cities;

    public int currentBatteryIndex = 0;

    // Click handling
    private static readonly float CLICK_DISTANCE_THRESHOLD = 0.5f;
    private bool isClickDown = false;
    private Vector2 clickDownPosition = Vector2.zero;

    private void Awake() {
        var spawnedPlayerData = playerSpawner.performInitialSpawn();
        missileBatteries = spawnedPlayerData.missileBatteries;
        cities = spawnedPlayerData.cities;
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

    // handle interaction
    void Update() {
        if (Input.GetButton("Fire1") && !isClickDown) {
            isClickDown = true;
            clickDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        } else if (!Input.GetButton("Fire1") && isClickDown) {
            isClickDown = false;
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector3.Distance(clickDownPosition, currentPosition) < CLICK_DISTANCE_THRESHOLD) {
                onClick(currentPosition.x, currentPosition.y);
            }
        }
    }
}
