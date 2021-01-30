using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvacuationController : MonoBehaviour {
    public Evacuator evacuatorPrefab;
    public float evacuatorYOffset;

    [Range(0, 1)]
    public float evacIntervalVariance = 0.33f;

    private GameState gameState;
    private WorldCoords worldCoords;
    private bool evacuating;
    private Coroutine evacLoop;

    private List<City> cities {
        get {
            return gameState.cities;
        }
    }
    private int currentCityIndex;

    internal void initialise(GameState gameState, WorldCoords worldCoords) {
        currentCityIndex = 0;
        this.gameState = gameState;
        this.worldCoords = worldCoords;
    }

    internal void clear() {
        currentCityIndex = 0;
        evacuating = false;
        if (evacLoop != null) {
            StopCoroutine(evacLoop);
        }
    }

    internal void beginEvacuations() {
        foreach (var city in cities) {
            if (!city.isDestroyed) city.populateEvacuees(worldCoords);
        }
        evacuating = true;
        evacLoop = StartCoroutine(startEvacuationLoop(gameState.evacEventInterval, evacIntervalVariance, false));
    }

    internal void completeEvacuations() {
        if (evacLoop != null) {
            StopCoroutine(evacLoop);
        }
        evacLoop = StartCoroutine(startEvacuationLoop(0.1f, 0, true));
    }

    private IEnumerator startEvacuationLoop(float averageInterval, float intervalVariance, bool boosted) {
        while (evacuating) {
            var wait = getVariated(averageInterval, intervalVariance);
            yield return new WaitForSeconds(wait);
            evacuating = performEvacuation(boosted);
        }
    }

    private float getVariated(float value, float variance) {
        float val = value * variance;
        return value - val + 2 * UnityEngine.Random.value * val;
    }

    private bool performEvacuation(bool boosted) {
        City evacCity = null;
        int initialIndex = currentCityIndex;
        for (int i = 0; i < cities.Count; i++) {
            evacCity = cities[(initialIndex + i) % cities.Count];
            currentCityIndex++;
            if (evacCity.canEvacuate()) break;
        }
        if (evacCity == null || !evacCity.canEvacuate()) {
            return false;
        }

        evacCity.evacuate(
            (evacueeCount) => gameState.onPopulationEvacuated(evacueeCount),
            (evacueeCount) => gameState.onPopulationLost(evacueeCount),
            boosted
        );
        return true;
    }
}
