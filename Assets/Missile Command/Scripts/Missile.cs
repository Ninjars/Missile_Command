using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
    public float missileLayerZ = 8;
    public float launchImpulse = 20;
    public float thrust = 10;
    public MissileExplosion explosionPrefab;
    public GameObject targetMarker;
    private TargetMarker marker;
    private Vector2 target;
    private Vector2 facing;

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
        Debug.Log($"OnCollisionEnter2D {gameObject.name} -> {other.gameObject.name} on layer {LayerMask.LayerToName(other.gameObject.layer)}");
        explode();
    }

    public void explode() {
        Debug.Log("explode()");
        gameObject.SetActive(false);

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<MissileExplosion>();
        explosion.boom(rb.position);
        
        marker.gameObject.SetActive(false);
        marker = null;
    }

    public void launch(Vector2 position, Vector2 target) {
        Debug.Log($"launch {gameObject.name}: {position} -> {target}");
        this.target = target;
        rb.position = position;
        this.facing = (target - position).normalized;
        transform.position = new Vector3(position.x, position.y, missileLayerZ);

        this.marker = ObjectPoolManager.Instance.getObjectInstance(targetMarker.gameObject).GetComponent<TargetMarker>();
        marker.configure(this, target);

        gameObject.SetActive(true);
        rb.AddForce(facing * launchImpulse, ForceMode2D.Impulse);
    }

    private void FixedUpdate() {
        rb.AddForce(facing * thrust, ForceMode2D.Force);
    }

    private void OnDrawGizmos() {
        Debug.DrawLine(
            new Vector3(rb.position.x, rb.position.y, missileLayerZ),
            new Vector3(target.x, target.y, missileLayerZ),
            Color.green
        );
    }

    private void OnDisable() {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }
}
