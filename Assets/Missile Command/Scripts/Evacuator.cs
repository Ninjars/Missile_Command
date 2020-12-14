using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evacuator : MonoBehaviour {
    public float zPos = 0;
    public float impulse = 10f;
    public float boostFactor = 5;
    public float descendToWorldY = -0.1f;

    private bool evacuating = false;
    private bool boosted = false;
    private WorldCoords worldCoords;
    private Action<long> onEvacComplete;
    private Action<long> onKilled;
    private long evacueeCount;
    private Rigidbody2D _rb;

    private Rigidbody2D rb {
        get {
            if (_rb == null) {
                _rb = GetComponent<Rigidbody2D>();
            }
            return _rb;
        }
    }

    internal void spawn(
        WorldCoords worldCoords,
        Vector2 startPosition,
        long evacueeCount
    ) {
        this.worldCoords = worldCoords;
        this.evacueeCount = evacueeCount;
        evacuating = false;
        boosted = false;
        transform.position = (Vector3) startPosition + Vector3.forward * zPos;

        rb.isKinematic = true;
        gameObject.SetActive(true);
    }

    internal void dispatch(
        Action<long> onEvacComplete,
        Action<long> onKilled,
        bool boosted
    ) {
        this.onEvacComplete = onEvacComplete;
        this.onKilled = onKilled;
        evacuating = true;
        this.boosted = boosted;
        propel(Vector2.down);
    }

    private void propel(Vector2 direction) {
        rb.isKinematic = false;
        rb.angularVelocity = 0;
        rb.velocity = Vector2.zero;
        var speed = boosted ? impulse * boostFactor : impulse;
        rb.AddForce(direction * speed);
    }

    private Vector2 calcXImpulse(WorldCoords worldCoords, Vector2 position) {
        if (position.x <= worldCoords.centerX) {
            return Vector2.right;
        } else {
            return Vector2.left;
        }
    }

    public void boost() {
        boosted = true;
        rb.velocity = rb.velocity * boostFactor;
    }

    private void Update() {
        if (!evacuating) return;

        if (rb.velocity.y < 0 && rb.position.y < descendToWorldY) {
            propel(calcXImpulse(worldCoords, rb.position));

        } else if (rb.position.y < worldCoords.worldBottom) {
            deliver();

        } else if (Mathf.Abs(rb.position.x) < 0.05f) {
            propel(Vector2.down);
        }
    }

    void OnDisable() {
        onEvacComplete = null;
        onKilled = null;
    }

    internal void deliver() {
        if (onEvacComplete != null) {
            onEvacComplete(evacueeCount);
        }
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!evacuating) return;
        
        if (onKilled != null) {
            onKilled(evacueeCount);
        }
        gameObject.SetActive(false);
    }
}
