using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvacuationController : MonoBehaviour {
    public Evacuator evacuatorPrefab;
    public float evacuatorYOffset;

    [Range(0, 1)]
    public float evacIntervalVariance = 0.33f;
    [Range(0, 1)]
    public float evacCountVariance = 0.33f;

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
        evacuating = true;
        evacLoop = StartCoroutine(startEvacuationLoop(gameState.evacEventInterval, gameState.evacEventCount));
    }

    internal void suspendEvacuations() {
        evacuating = false;
        if (evacLoop != null) {
            StopCoroutine(evacLoop);
        }
    }

    private IEnumerator startEvacuationLoop(float averageInterval, long averageCount) {
        while (evacuating) {
            performEvacuation((long) getVariated(averageCount, evacCountVariance));
            yield return new WaitForSeconds(getVariated(averageInterval, evacIntervalVariance));
        }
    }

    private float getVariated(float value, float variance) {
        float val = value * variance;
        return value - val + 2 * UnityEngine.Random.value * val;
    }

    private void performEvacuation(long maxEvacCount) {
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

        long evacCount = evacCity.evacuate(maxEvacCount);
        Evacuator evacuator = ObjectPoolManager.Instance.getObjectInstance(evacuatorPrefab.gameObject).GetComponent<Evacuator>();
        evacuator.dispatch(
            worldCoords,
            new Vector2(evacCity.transform.position.x, evacuatorYOffset),
            (evacueeCount) => gameState.onPopulationEvacuated(evacueeCount),
            (evacueeCount) => gameState.onPopulationLost(evacueeCount),
            evacCount
        );
    }
}
