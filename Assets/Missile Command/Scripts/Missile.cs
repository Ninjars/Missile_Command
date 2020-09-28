using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
    public float missileLayerZ = 8;
    public float thrust = 10;
    private Rigidbody2D _rb;
    private Rigidbody2D rb {
        get {
            if (_rb == null) {
                _rb = GetComponent<Rigidbody2D>();
            }
            return _rb;
        }
    }

    public void launch(Vector2 position, Vector2 target) {
        Debug.Log($"launch {gameObject.name}: {position} -> {target}");
        rb.position = position;
        transform.position = new Vector3(rb.position.x, rb.position.y, missileLayerZ);
        gameObject.SetActive(true);
        var vector = (target - rb.position).normalized;
        rb.AddForce(vector * thrust, ForceMode2D.Impulse);
    }

    private void OnDisable() {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }
}
