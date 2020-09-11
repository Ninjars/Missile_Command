using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerSpawner playerSpawner;

    private List<MissileBattery> missileBatteries;
    private List<City> cities;
    
    private void Awake() {
        var spawnedPlayerData = playerSpawner.performInitialSpawn();
        missileBatteries = spawnedPlayerData.missileBatteries;
        cities = spawnedPlayerData.cities;
    }
}
