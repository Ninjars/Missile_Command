using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;

public class City : Explodable {
    public bool isDestroyed = false;
    public GameObject aliveVisuals;
    public GameObject deadVisuals;
    public CityUpgradeUI upgradeUi;
    public Evacuator evacuatorPrefab;
    public Explosion explosionPrefab;
    public CityUI uiController;
    public float evacuatorYOffset = 0.1f;
    public float evacuatorXSpacing = 0.11f;

    public long population { get; private set; }
    public CityUpgradeState upgradeState { get; private set; }
    private StateUpdater stateUpdater;
    private CityEvacuationStats evacuationStats;
    private Colors colors { get { return Colors.Instance; } }
    private ScreenEffectManager _screenEffectManager;
    private ScreenEffectManager screenEffectManager {
        get {
            if (_screenEffectManager == null) {
                _screenEffectManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScreenEffectManager>();
            }
            return _screenEffectManager;
        }
    }
    private List<Evacuator> evacuators;

    public void initialise(StateUpdater stateUpdater, long population, int evacEventCount, long popPerEvac) {
        this.stateUpdater = stateUpdater;
        this.population = population;
        upgradeState = new CityUpgradeState();
        evacuationStats = new CityEvacuationStats(evacEventCount, popPerEvac, upgradeState);
        evacuators = new List<Evacuator>();

        uiController.initialise(gameObject.name, population);

        aliveVisuals.GetComponent<Polyline>().Color = colors.cityColor;
        deadVisuals.GetComponent<Polyline>().Color = colors.deadBuildingColor;
        hideUpgradeOptions();
    }

    public void showUi(bool isUpgrading) {
        uiController.updateCityPopReadoutContent(population);
        uiController.display(isUpgrading);
    }

    public void hideUi() {
        uiController.onHide(false);
    }

    public void fadeOutUi() {
        uiController.onHide(true);
    }

    public long evacuate(
        Action<long> onEvacComplete,
        Action<long> onKilled,
        bool boosted
    ) {
        if (isDestroyed || !evacuationStats.canEvacuate()) {
            return 0;
        }

        evacuationStats.evacuationPerformed();

        long evacCount = population < evacuationStats.popPerEvent ? population : evacuationStats.popPerEvent;
        population -= evacCount;

        var evacuator = evacuators[0];
        evacuators.RemoveAt(0);
        evacuator.dispatch(
            onEvacComplete,
            onKilled,
            boosted
        );

        if (population <= 0) {
            aliveVisuals.GetComponent<Polyline>().Color = colors.deadBuildingColor;
        }
        uiController.updateCityPopReadoutContent(population);
        
        return evacCount;
    }

    internal void populateEvacuees(WorldCoords worldCoords) {
        evacuationStats.refresh();

        foreach (var evacuator in evacuators) {
            evacuator.gameObject.SetActive(false);
        }
        evacuators.Clear();
        
        var evacuatorPool = ObjectPoolManager.Instance.getObjectPool(evacuatorPrefab.gameObject);
        var totalWidth = evacuatorXSpacing * (evacuationStats.eventsPerLevel - 1);
        var xOrigin = -totalWidth * 0.5f;
        for (int i = 0; i < evacuationStats.eventsPerLevel; i++) {
            Evacuator evac = evacuatorPool.getObjectInstance().GetComponent<Evacuator>();
            evac.spawn(
                worldCoords,
                transform.position + Vector3.up * evacuatorYOffset + Vector3.right * (xOrigin + i * evacuatorXSpacing),
                evacuationStats.popPerEvent
            );
            evacuators.Add(evac);
        }
        // reverse list to make it easier to read off right-most item
        evacuators.Reverse();
    }

    internal bool canEvacuate() {
        return !isDestroyed && population > 0 && evacuationStats.canEvacuate();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!isDestroyed) {
            destroy();
        }
        var explodable = other.GetComponent<Explodable>();
        if (explodable != null) {
            explodable.explode();
        }
    }

    public override void explode() {
        destroy();
    }

    private void destroy() {
        if (isDestroyed) return;
        GetComponent<CircleCollider2D>().enabled = false;
        stateUpdater.onPopulationLost(population);
        population = 0;
        isDestroyed = true;
        screenEffectManager.onCityNukeHit(1f);
        evacuationStats.clear();
        hideUpgradeOptions();

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(transform.position, colors.buildingExplodeColor);
        foreach (var evacuator in evacuators) {
            evacuator.gameObject.SetActive(false);
        }
        StartCoroutine(setDeadVisuals());

        uiController.onDead();
    }

    private IEnumerator setDeadVisuals() {
        yield return new WaitForSeconds(0.2f);

        aliveVisuals.SetActive(false);
        deadVisuals.SetActive(true);
    }

    #region Upgrades
    public void showUpgradeOptions(Action onHighlightCallback, Action onUpgradeCallback) {
        if (isDestroyed) return;
        upgradeUi.gameObject.SetActive(true);
        upgradeUi.registerCallbacks(onHighlightCallback, onUpgradeCallback);
    }

    public void hideUpgradeOptions() {
        upgradeUi.gameObject.SetActive(false);
    }

    public void deselectUpgradeUi() {
        if (isDestroyed) return;
        upgradeUi.onDeselect();
    }
    #endregion
}

public class CityEvacuationStats {
    private readonly CityUpgradeState upgradeState;
    private readonly int baseEventsPerLevel;
    private readonly long basePopPerEvent;
    public int eventsPerLevel { get { return baseEventsPerLevel + upgradeState.evacuatorCount; } }
    public long popPerEvent { get { return Convert.ToInt64(basePopPerEvent * upgradeState.evacuatorPopFactor); } }
    public int eventsRemaining { get; private set; }

    public CityEvacuationStats(int evacEventCount, long popPerEvac, CityUpgradeState upgradeState) {
        this.upgradeState = upgradeState;
        baseEventsPerLevel = evacEventCount;
        basePopPerEvent = popPerEvac;
    }

    public void refresh() {
        eventsRemaining = eventsPerLevel;
    }

    public bool canEvacuate() {
        return eventsRemaining > 0;
    }

    public void evacuationPerformed() {
        eventsRemaining--;
    }

    public void clear() {
        eventsRemaining = 0;
    }
}
