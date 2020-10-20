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
    public int maxMissiles = 10;
    public int missilesStored = 10;
    public float missileLaunchOffset = 0.3f;

    public int ammoPerRow = 10;
    public float maxXAmmoOffset = 0.5f;
    public float yAmmoOffset = 0.1f;
    public float ammoPadding = 0.01f;
    public float fullAmmoHeight = 0.4f;

    private Colors colors { get { return Colors.Instance; }}
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
        missile.launch(transform.position + Vector3.up * missileLaunchOffset, new Vector2(x, y));

        updateAmmoIndicators();
        return true;
    }

    internal void destroy() {
        if (isDestroyed) return;

        GetComponent<BoxCollider2D>().enabled = false;
        isDestroyed = true;
        missilesStored = 0;
        updateAmmoIndicators();
        screenEffectManager.onBatteryDestroyed();

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
        missilesStored = maxMissiles;
        updateAmmoIndicators();
        lineShape.Color = colors.batteryColor;
        GetComponent<BoxCollider2D>().enabled = true;
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
        foreach (var obj in ammoIndicators) {
            GameObject.Destroy(obj.gameObject);
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
        float y = yAmmoOffset - height - row * (height + ammoPadding);

        Rectangle indicator = GameObject.Instantiate(ammoIndicatorPrefab, transform, false).GetComponent<Rectangle>();
        indicator.transform.position = transform.position + new Vector3(x, y, 0);
        indicator.Width = width;
        indicator.Height = height;
        indicator.gameObject.SetActive(true);

        return indicator;
    }
}
