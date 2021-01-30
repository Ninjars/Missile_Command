using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;

public class City : MonoBehaviour {
    public float uiFadeDuration = 2f;
    public bool isDestroyed = false;
    public GameObject aliveVisuals;
    public GameObject deadVisuals;
    public GameObject textUi;
    public Evacuator evacuatorPrefab;
    public TextMeshProUGUI cityNameView;
    public TextMeshProUGUI cityPopulationView;
    public Explosion explosionPrefab;
    public float evacuatorYOffset = 0.1f;
    public float evacuatorXSpacing = 0.11f;

    public long population { get; private set; }
    public CityUpgradeState upgradeState { get; private set; }
    private StateUpdater stateUpdater;
    private CityEvacuationStats evacuationStats;
    private Colors colors { get { return Colors.Instance; } }
    private Triangle markerTriangle;
    private CanvasGroup textCanvasGroup;
    private bool isFadingUi;
    private float fadeStart;
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

        cityNameView.text = gameObject.name;
        cityPopulationView.text = $"{population}";

        aliveVisuals.GetComponent<Polyline>().Color = colors.cityColor;
        markerTriangle = textUi.GetComponent<Triangle>();
        textCanvasGroup = textUi.GetComponentInChildren<CanvasGroup>();
        deadVisuals.GetComponent<Polyline>().Color = colors.deadBuildingColor;
    }

    public void showUi() {
        isFadingUi = false;
        textCanvasGroup.alpha = 1;
        Color color = isDestroyed ? colors.deadBuildingColor : colors.textColor;
        markerTriangle.Color = color;
        cityNameView.color = color;
        cityPopulationView.color = color;
        cityPopulationView.text = isDestroyed ? "DEAD" : $"{population}";
        textUi.SetActive(true);
    }

    public void hideUi() {
        isFadingUi = false;
        textUi.SetActive(false);
    }

    public void fadeOutUi() {
        if (!textUi.activeInHierarchy) return;
        isFadingUi = true;
        fadeStart = Time.time;
    }

    private void Update() {
        if (isFadingUi) {
            float fraction = (Time.time - fadeStart) / uiFadeDuration;
            if (fraction <= 1) {
                Color color = markerTriangle.Color;
                color.a = Mathf.Lerp(color.a, 0, fraction * fraction);
                markerTriangle.Color = color;
                textCanvasGroup.alpha = color.a;

            } else {
                textUi.SetActive(false);
                isFadingUi = false;
            }
        }
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
        other.gameObject.SendMessage("explode");
    }

    private void destroy() {
        if (isDestroyed) return;
        GetComponent<CircleCollider2D>().enabled = false;
        stateUpdater.onPopulationLost(population);
        population = 0;
        isDestroyed = true;
        screenEffectManager.onCityNukeHit(1f);
        evacuationStats.clear();

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(transform.position, colors.buildingExplodeColor);
        foreach (var evacuator in evacuators) {
            evacuator.gameObject.SetActive(false);
        }
        StartCoroutine(setDeadVisuals());

        if (isFadingUi) {
            showUi();
            fadeOutUi();
        }
    }

    private IEnumerator setDeadVisuals() {
        yield return new WaitForSeconds(0.2f);

        aliveVisuals.SetActive(false);
        deadVisuals.SetActive(true);
    }
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
