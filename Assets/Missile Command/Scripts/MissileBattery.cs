using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class MissileBattery : MonoBehaviour {
    public GameObject missilePrefab;
    public GameObject ammoIndicatorPrefab;
    public GameObject loadedIndicator;
    public Explosion explosionPrefab;
    public Color aliveColor;
    public Color deadColor;
    public int maxMissiles = 10;
    public int missilesStored = 10;

    public int ammoPerRow = 10;
    public float maxXAmmoOffset = 0.5f;
    public float ammoPadding = 0.01f;
    public float fullAmmoHeight = 0.4f;

    private bool isDestroyed = false;
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
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log($"OnTriggerEnter2D {gameObject.name} -> {other.gameObject.name} on layer {LayerMask.LayerToName(other.gameObject.layer)}");
        destroy();
        other.gameObject.SendMessage("explode");
    }

    public bool fire(float x, float y) {
        if (missilesStored <= 0 || isDestroyed) {
            return false;
        }

        missilesStored--;
        var missile = ObjectPoolManager.Instance.getObjectInstance(missilePrefab).GetComponent<Missile>();
        missile.launch(transform.position, new Vector2(x, y));

        updateAmmoIndicators();
        return true;
    }

    internal bool getIsDestroyed() {
        return isDestroyed;
    }

    internal void destroy() {
        if (isDestroyed) return;

        isDestroyed = true;
        missilesStored = 0;
        updateAmmoIndicators();
        screenEffectManager.onBatteryDestroyed();

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(transform.position);
        StartCoroutine(setDeadVisuals());
    }

    private IEnumerator setDeadVisuals() {
        yield return new WaitForSeconds(0.2f);

        lineShape.Color = deadColor;
    }

    internal void restore() {
        isDestroyed = false;
        missilesStored = maxMissiles;
        updateAmmoIndicators();
        lineShape.Color = aliveColor;
    }

    private void updateAmmoIndicators() {
        if (ammoIndicators.Count != maxMissiles) {
            regenerateAmmoIndicators();
        }

        for (int i = 0; i < ammoIndicators.Count; i++) {
            if (i < missilesStored) {
                ammoIndicators[i].gameObject.SetActive(true);
            } else {
                ammoIndicators[i].gameObject.SetActive(false);
            }
        }
        loadedIndicator.SetActive(missilesStored > 0);
    }

    private void regenerateAmmoIndicators() {
        Debug.Log($"regenerateAmmoIndicators()");
        foreach (var obj in ammoIndicators) {
            obj.gameObject.SetActive(false);
        }
        ammoIndicators.Clear();

        float ammoWidth = (maxXAmmoOffset * 2 - ((ammoPerRow - 1) * ammoPadding)) / ammoPerRow;
        for (int i = 0; i < maxMissiles; i++) {
            ammoIndicators.Add(createAmmoIndicator(ammoWidth, i % ammoPerRow, i / ammoPerRow, maxMissiles > ammoPerRow));
        }
    }

    private Rectangle createAmmoIndicator(float width, int column, int row, bool compressed) {
        float height = compressed ? fullAmmoHeight / 2f : fullAmmoHeight;
        float x = -maxXAmmoOffset + column * (width + ammoPadding);
        float y = -1 * (ammoPadding + (height * 0.5f) + row * (height + ammoPadding));

        Rectangle indicator = ObjectPoolManager.Instance.getObjectInstance(ammoIndicatorPrefab.gameObject).GetComponent<Rectangle>();
        indicator.transform.position = transform.position + new Vector3(x, y, 0);
        indicator.Width = width;
        indicator.Height = height;
        indicator.gameObject.SetActive(true);

        return indicator;
    }
}
