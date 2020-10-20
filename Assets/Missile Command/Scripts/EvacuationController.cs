using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvacuationController : MonoBehaviour {
    public Evacuator evacuatorPrefab;
    public float evacuatorYOffset;

    private GameState gameState;
    private WorldCoords worldCoords;

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
