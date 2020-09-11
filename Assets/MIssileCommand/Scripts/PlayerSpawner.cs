using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public MissileBattery batteryPrefab;
    public int batteryMaxOffset;
    public int batteryCount;
    public float batteryZPos;

    public City cityPrefab;
    public float cityXOffsetFactor;
    public float cityYPos;
    public float cityZPos;
    
    public SpawnData performInitialSpawn()
    {
        List<MissileBattery> missileBatteries = new List<MissileBattery>();
        List<City> cities = new List<City>();

        List<float> positions = new List<float>();
        for (int i = 0; i < batteryCount; i++)
        {
            positions.Add(getMissileBatteryPosition(i));
        }

        float cityOffset;
        if (positions.Count <= 1) {
            cityOffset = 1;
        } else {
            cityOffset = Mathf.Abs(positions[1] - positions[0]) * cityXOffsetFactor;
        }
        for (int i = 0; i < positions.Count; i++) 
        {
            var position = positions[i];
            var missileBattery = GameObject.Instantiate(batteryPrefab);
            missileBattery.transform.position = new Vector3(position, 0, batteryZPos);
            missileBattery.gameObject.name = $"MissileBattery {i}";
            missileBatteries.Add(missileBattery);

            var cityA = instantiateCity(position + cityOffset);
            cityA.gameObject.name = $"City {i}A";
            cities.Add(cityA);

            var cityB = instantiateCity(position - cityOffset);
            cityB.gameObject.name = $"City {i}B";
            cities.Add(cityB);
        }

        return new SpawnData(missileBatteries, cities);
    }

    private City instantiateCity(float xPos) {
        var city = GameObject.Instantiate(cityPrefab);
        city.transform.position = new Vector3(xPos, cityYPos, cityZPos);
        return city;
    }

    private float getMissileBatteryPosition(int index)
    {
        float fraction = Mathf.Ceil(index * 0.5f) / (batteryCount * 0.5f);
        if (index % 2 != 0) fraction *= -1;
        return fraction * batteryMaxOffset;
    }
}

public class SpawnData
{
    public readonly List<MissileBattery> missileBatteries;
    public readonly List<City> cities;

    public SpawnData(List<MissileBattery> missileBatteries, List<City> cities)
    {
        this.missileBatteries = missileBatteries;
        this.cities = cities;
    }
}
