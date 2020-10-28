using System;
using System.Collections.Generic;
using System.Linq;
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

        float batterySpacing = worldCoords.width / batteryCount;
        float cityOffset = batterySpacing / 3f;

        List<float> citySpawnPositions = new List<float>(batteryCount * 2);

        for (int i = 0; i < batteryCount; i++) {
            var batteryPosition = batterySpacing * (i + 0.5f) + worldCoords.worldLeft;
            var missileBattery = GameObject.Instantiate(batteryPrefab);
            missileBattery.transform.position = new Vector3(batteryPosition, batteryYPos, batteryZPos);
            missileBattery.gameObject.name = $"MissileBattery {i}";
            missileBatteries.Add(missileBattery);

            citySpawnPositions.Add(batteryPosition - cityOffset);
            citySpawnPositions.Add(batteryPosition + cityOffset);
        }

        CityNameData cityNameData = CityDataProvider.getCityNames(citySpawnPositions.Count);
        List<City> cities = new List<City>(citySpawnPositions.Count);
        for (int i = 0; i < citySpawnPositions.Count; i++) {
            var city = instantiateCity(citySpawnPositions[i]);
            city.gameObject.name = cityNameData.cityNames[i];
            city.initialise(stateUpdater, initialPopulation);
            cities.Add(city);
        }

        return new SpawnData(missileBatteries, cities, cityNameData.countryName);
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
    public readonly string countryName;

    public SpawnData(List<MissileBattery> missileBatteries, List<City> cities, string countryName) {
        this.missileBatteries = missileBatteries;
        this.cities = cities;
        this.countryName = countryName;
    }
}
