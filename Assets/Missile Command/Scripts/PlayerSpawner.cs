using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    public MissileBattery batteryPrefab;
    public int batteryCount;
    public float batteryYPos;
    public float batteryZPos;

    public City cityPrefab;
    public float cityYPos;
    public float cityZPos;

    public SpawnData performInitialSpawn(StateUpdater stateUpdater, WorldCoords worldCoords, int numberOfLevels, long initialPopulation) {
        List<MissileBattery> missileBatteries = new List<MissileBattery>();
        List<City> cities = new List<City>();

        float batterySpacing = worldCoords.width / (batteryCount + 1f);
        float cityOffset = batterySpacing / 3f;

        for (int i = 0; i < batteryCount; i++) {
            var batteryPosition = batterySpacing * (i + 1) + worldCoords.worldLeft;
            var missileBattery = GameObject.Instantiate(batteryPrefab);
            missileBattery.transform.position = new Vector3(batteryPosition, batteryYPos, batteryZPos);
            missileBattery.gameObject.name = $"MissileBattery {i}";
            missileBatteries.Add(missileBattery);

            var cityA = instantiateCity(batteryPosition - cityOffset);
            cityA.gameObject.name = $"City {i}A";
            cityA.initialise(stateUpdater, initialPopulation);
            cities.Add(cityA);

            var cityB = instantiateCity(batteryPosition + cityOffset);
            cityB.gameObject.name = $"City {i}B";
            cityB.initialise(stateUpdater, initialPopulation);
            cities.Add(cityB);
        }

        return new SpawnData(missileBatteries, cities);
    }

    private City instantiateCity(float xPos) {
        var city = GameObject.Instantiate(cityPrefab);
        city.transform.position = new Vector3(xPos, cityYPos, cityZPos);
        return city;
    }
}

public class SpawnData {
    public readonly List<MissileBattery> missileBatteries;
    public readonly List<City> cities;

    public SpawnData(List<MissileBattery> missileBatteries, List<City> cities) {
        this.missileBatteries = missileBatteries;
        this.cities = cities;
    }
}
