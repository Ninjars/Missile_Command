using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
    public float missileLayerZ = 8;
    public float launchImpulse = 20;
    public float thrust = 10;
    public float rateOfTurnDegrees = 3;
    public MissileExplosion explosionPrefab;
    private Vector2 facing;
    private Vector2 target;

    private Rigidbody2D _rb;
    private Rigidbody2D rb {
        get {
            if (_rb == null) {
                _rb = GetComponent<Rigidbody2D>();
            }
            return _rb;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {

    }

    public void launch(Vector2 position, Vector2 target) {
        Debug.Log($"launch {gameObject.name}: {position} -> {target}");
        this.target = target;
        rb.position = position;
        facing = (target - position).normalized;
        transform.position = new Vector3(position.x, position.y, missileLayerZ);
        gameObject.SetActive(true);
        rb.AddForce(facing * launchImpulse, ForceMode2D.Impulse);
    }

    private void FixedUpdate() {
        rb.AddForce(facing * thrust, ForceMode2D.Force);
    }

    private void OnDrawGizmos() {
        Debug.DrawLine(
            new Vector3(rb.position.x, rb.position.y, missileLayerZ),
            new Vector3(rb.position.x + facing.x, rb.position.y + facing.y, missileLayerZ),
            Color.red
        );
        Debug.DrawLine(
            new Vector3(rb.position.x, rb.position.y, missileLayerZ),
            new Vector3(target.x, target.y, missileLayerZ),
            Color.green
        );
    }

    private static Vector2 rotate(Vector2 v, float delta) {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    private void OnDisable() {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }
}
