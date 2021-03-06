﻿using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissileBattery : Explodable {
    public GameObject missilePrefab;
    public GameObject ammoIndicatorPrefab;
    public GameObject loadedIndicator;
    public BatteryUpgradeUI upgradeUi;
    public BatteryUI batteryUi;
    public Explosion explosionPrefab;
    public int baseMaxMissiles = 10;
    public float baseMissileSpeed = 5;
    public float baseMissileExplosionRadius = 0.5f;
    public float baseMissileExplosionDuration = 1f;
    public float missileLaunchOffset = 0.3f;

    public int ammoPerRow = 10;
    public float maxXAmmoOffset = 0.5f;
    public float yAmmoOffset = 0.1f;
    public float ammoPadding = 0.01f;
    public float fullAmmoHeight = 0.4f;

    public BatteryUpgradeState upgradeState { get; private set; }
    private MissileBatteryStats stats;

    private Colors colors { get { return Colors.Instance; } }
    public bool isDestroyed { get; private set; }
    private List<Rectangle> ammoIndicators;
    private Polyline lineShape;
    private ScreenEffectManager _screenEffectManager;
    private ScreenEffectManager screenEffectManager {
        get {
            if (_screenEffectManager == null) {
                _screenEffectManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScreenEffectManager>();
            }
            return _screenEffectManager;
        }
    }

    private void Awake() {
        ammoIndicators = new List<Rectangle>();
        lineShape = GetComponentInChildren<Polyline>();

        upgradeState = new BatteryUpgradeState();
        stats = new MissileBatteryStats(
            upgradeState,
            baseMaxMissiles,
            baseMissileSpeed,
            baseMissileExplosionRadius,
            baseMissileExplosionDuration
        );
        hideUpgradeOptions();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        destroy();
        var explodable = other.GetComponent<Explodable>();
        if (explodable != null) {
            explodable.explode();
        }
    }

    public override void explode() {
        destroy();
    }

    public bool fire(float x, float y) {
        if (!stats.canLaunchMissile() || isDestroyed) {
            return false;
        }

        stats.onMissileLaunched();
        var missile = ObjectPoolManager.Instance.getObjectInstance(missilePrefab).GetComponent<Missile>();
        missile.launch(
            transform.position + Vector3.up * missileLaunchOffset, 
            new Vector2(x, y),
            stats.missileSpeed,
            stats.explosionRadius,
            stats.explosionDuration
        );

        updateAmmoIndicators();
        return true;
    }

    internal void destroy() {
        if (isDestroyed) return;

        GetComponent<BoxCollider2D>().enabled = false;
        isDestroyed = true;
        stats.clear();
        updateAmmoIndicators();
        screenEffectManager.onBatteryDestroyed();
        batteryUi.setIsDead(true);

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(transform.position, colors.buildingExplodeColor);
        StartCoroutine(setDeadVisuals());
    }

    private IEnumerator setDeadVisuals() {
        yield return new WaitForSeconds(0.2f);

        lineShape.Color = colors.deadBuildingColor;
    }

    internal void restore() {
        isDestroyed = false;
        batteryUi.setIsDead(false);
        stats.refresh();
        updateAmmoIndicators();
        lineShape.Color = colors.batteryColor;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    public void showLabel() {
        batteryUi.displayLabel();
    }

    public void showUpgradeIndicator() {
        batteryUi.displayIndicator();
    }

    public void hideUi() {
        batteryUi.onHide();
    }

    private void updateAmmoIndicators() {
        if (ammoIndicators.Count != stats.maxMissiles) {
            regenerateAmmoIndicators();
        }

        for (int i = 0; i < ammoIndicators.Count; i++) {
            if (i < stats.remainingMissiles) {
                ammoIndicators[i].gameObject.SetActive(true);
            } else {
                ammoIndicators[i].gameObject.SetActive(false);
            }
        }
        loadedIndicator.SetActive(stats.canLaunchMissile());
    }

    private void regenerateAmmoIndicators() {
        foreach (var obj in ammoIndicators) {
            GameObject.Destroy(obj.gameObject);
        }
        ammoIndicators.Clear();

        float ammoWidth = (maxXAmmoOffset * 2 - ((ammoPerRow - 1) * ammoPadding)) / ammoPerRow;
        int maxMissiles = stats.maxMissiles;
        for (int i = 0; i < maxMissiles; i++) {
            ammoIndicators.Add(createAmmoIndicator(ammoWidth, i % ammoPerRow, i / ammoPerRow, maxMissiles > ammoPerRow));
        }
    }

    private Rectangle createAmmoIndicator(float width, int column, int row, bool compressed) {
        float height = compressed ? fullAmmoHeight / 2f : fullAmmoHeight;
        float x = -maxXAmmoOffset + column * (width + ammoPadding);
        float y = yAmmoOffset - height - row * (height + ammoPadding);

        Rectangle indicator = GameObject.Instantiate(ammoIndicatorPrefab, transform, false).GetComponent<Rectangle>();
        indicator.transform.position = transform.position + new Vector3(x, y, 0);
        indicator.Width = width;
        indicator.Height = height;
        indicator.gameObject.SetActive(true);

        return indicator;
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

public class MissileBatteryStats {
    private readonly BatteryUpgradeState upgradeState;
    private readonly int baseMaxMissileCount;
    private readonly float baseMissileSpeed;
    private readonly float baseExplosionRadius;
    private readonly float baseExplosionDuration;
    public int remainingMissiles { get; private set; }
    public int maxMissiles { get { return baseMaxMissileCount; } }
    public float missileSpeed { get { return baseMissileSpeed * upgradeState.missileSpeedFactor; } }
    public float explosionRadius { get { return baseExplosionRadius * upgradeState.explosionRadiusFactor; } }
    public float explosionDuration { get { return baseExplosionDuration * upgradeState.explosionLingerFactor; } }

    public MissileBatteryStats(BatteryUpgradeState upgradeState, int baseMaxMissileCount, float baseMissileSpeed, float baseExplosionRadius, float baseExplosionDuration) {
        this.upgradeState = upgradeState;
        this.baseMaxMissileCount = baseMaxMissileCount;
        this.baseMissileSpeed = baseMissileSpeed;
        this.baseExplosionRadius = baseExplosionRadius;
        this.baseExplosionDuration = baseExplosionDuration;
    }

    public void clear() {
        remainingMissiles = 0;
    }

    public void refresh() {
        remainingMissiles = maxMissiles;
    }

    public void onMissileLaunched() {
        remainingMissiles--;
    }

    public bool canLaunchMissile() {
        return remainingMissiles > 0;
    }
}
