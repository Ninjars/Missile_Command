using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBattery : MonoBehaviour {
    public GameObject missilePrefab;
    public int missilesStored = 10;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public bool fire(float x, float y) {
        if (missilesStored <= 0) {
            return false;
        }

        missilesStored--;
        var missile = ObjectPoolManager.Instance.getObjectInstance(missilePrefab).GetComponent<Missile>();
        missile.launch(rb.position, new Vector2(x, y));
        return true;
    }
}
