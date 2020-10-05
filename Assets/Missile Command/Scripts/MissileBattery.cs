using System;
using UnityEngine;

public class MissileBattery : MonoBehaviour {
    public GameObject missilePrefab;
    public int maxMissiles = 10;
    public int missilesStored = 10;
    private bool isDestroyed = false;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public bool fire(float x, float y) {
        if (missilesStored <= 0 || isDestroyed) {
            return false;
        }
        Debug.Log($"{gameObject.name} firing - ammunition: {missilesStored}");

        missilesStored--;
        var missile = ObjectPoolManager.Instance.getObjectInstance(missilePrefab).GetComponent<Missile>();
        missile.launch(rb.position, new Vector2(x, y));
        return true;
    }

    internal bool getIsDestroyed() {
        return isDestroyed;
    }

    internal void setIsDestroyed(bool isDestroyed) {
        this.isDestroyed = isDestroyed;
    }
}
