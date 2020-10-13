using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvacuationController : MonoBehaviour {
    private List<City> cities;
    private GameState gameState;
    private int currentCityIndex;

    internal void initialise(List<City> cities, GameState gameState) {
        currentCityIndex = 0;
        this.cities = cities;
        this.gameState = gameState;
    }

    internal void performEvacuation() {
        City evacCity = null;
        int initialIndex = currentCityIndex;
        for (int i = 0; i < cities.Count; i++) {
            evacCity = cities[(initialIndex + i) % cities.Count];
            currentCityIndex++;
            if (evacCity.canEvacuate()) break;
        }
        if (evacCity == null || !evacCity.canEvacuate()) {
            return;
        }

        long evacCount = evacCity.evacuate();
        gameState.onPopulationEvacuated(evacCount);
        // TODO: visual indicating evacuation amount and location
        Debug.Log($"{evacCity.gameObject.name} evacuated {evacCount}; {evacCity.population} remaining");
    }
}
