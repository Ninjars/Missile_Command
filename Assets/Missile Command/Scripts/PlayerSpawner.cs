using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    public MissileBattery batteryPrefab;
    public int batteryCount;
    public float batteryZPos;

    public City cityPrefab;
    public long cityPopulation = 1000000;
    public float cityYPos;
    public float cityZPos;

    public SpawnData performInitialSpawn(WorldCoords worldCoords, int numberOfLevels, float evacuationFactor) {
        List<MissileBattery> missileBatteries = new List<MissileBattery>();
        List<City> cities = new List<City>();

        float worldWidth = worldCoords.worldRight - worldCoords.worldLeft;
        float batterySpacing = worldWidth / (batteryCount + 1f);
        float cityOffset = batterySpacing / 3f;

        long evacuationRate = calcEvacRate(batteryCount * 2, numberOfLevels, evacuationFactor);
        for (int i = 0; i < batteryCount; i++) {
            var batteryPosition = batterySpacing * (i + 1) + worldCoords.worldLeft;
            var missileBattery = GameObject.Instantiate(batteryPrefab);
            missileBattery.transform.position = new Vector3(batteryPosition, 0, batteryZPos);
            missileBattery.gameObject.name = $"MissileBattery {i}";
            missileBatteries.Add(missileBattery);

            var cityA = instantiateCity(batteryPosition + cityOffset);
            cityA.gameObject.name = $"City {i}A";
            cityA.initialise(cityPopulation, evacuationRate);
            cities.Add(cityA);

            var cityB = instantiateCity(batteryPosition - cityOffset);
            cityB.gameObject.name = $"City {i}B";
            cityB.initialise(cityPopulation, evacuationRate);
            cities.Add(cityB);
        }

        return new SpawnData(missileBatteries, cities);
    }
    
    private long calcEvacRate(int cityCount, int numberOfLevels, float evacuationFactor) {
        int numberOfEvacuationsPerCity = numberOfLevels / cityCount;
        return (long) Math.Ceiling(cityPopulation / numberOfEvacuationsPerCity * evacuationFactor);
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
