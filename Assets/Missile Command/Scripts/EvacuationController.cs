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

    internal void beginEvacuations() {
        foreach (var city in cities) {
            if (!city.isDestroyed) city.populateEvacuees(worldCoords);
        }
        evacuating = true;
        evacLoop = StartCoroutine(startEvacuationLoop(gameState.evacEventInterval));
    }

    internal void completeEvacuations() {
        evacuating = false;
        if (evacLoop != null) {
            StopCoroutine(evacLoop);
        }
    }

    private IEnumerator startEvacuationLoop(float averageInterval) {
        while (evacuating) {
            performEvacuation();
            var wait = getVariated(averageInterval, evacIntervalVariance);
            Debug.Log($"> waiting for avg {averageInterval} variance {evacIntervalVariance} {wait}");
            yield return new WaitForSeconds(wait);
        }
    }

    private float getVariated(float value, float variance) {
        float val = value * variance;
        return value - val + 2 * UnityEngine.Random.value * val;
    }

    private void performEvacuation() {
        Debug.Log("performEvacuation()");
        City evacCity = null;
        int initialIndex = currentCityIndex;
        for (int i = 0; i < cities.Count; i++) {
            evacCity = cities[(initialIndex + i) % cities.Count];
            currentCityIndex++;
            if (evacCity.canEvacuate()) break;
        }
        Debug.Log($"> evacCity: {evacCity}");
        if (evacCity == null || !evacCity.canEvacuate()) {
            return;
        }

        evacCity.evacuate(
            (evacueeCount) => gameState.onPopulationEvacuated(evacueeCount),
            (evacueeCount) => gameState.onPopulationLost(evacueeCount)
        );
    }
}
