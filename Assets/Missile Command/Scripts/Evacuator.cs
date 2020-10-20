using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evacuator : MonoBehaviour {
    public float zPos = 0;
    public float impulse = 10f;

    
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

    internal void dispatch(
        WorldCoords worldCoords, 
        Vector2 startPosition, 
        Action<long> onEvacComplete, 
        Action<long> onKilled, 
        long evacueeCount
    ) {
        this.worldCoords = worldCoords;
        this.onEvacComplete = onEvacComplete;
        this.onKilled = onKilled;
        this.evacueeCount = evacueeCount;
        transform.position = (Vector3) startPosition + Vector3.forward * zPos;

        Vector2 impulse = calcInitialImpulse(worldCoords, startPosition);
        if (impulse.x < 0) {
            transform.localScale = new Vector3(-1, 1, 1);
        } else {
            transform.localScale = Vector3.one;
        }

        gameObject.SetActive(true);
        rb.angularVelocity = 0;
        rb.velocity = Vector2.zero;
        rb.AddForce(impulse);
    }

    private Vector2 calcInitialImpulse(WorldCoords worldCoords, Vector2 position) {
        if (position.x <= worldCoords.centerX) {
            return Vector2.left * impulse;
        } else {
            return Vector2.right * impulse;
        }
    }

    private void Update() {
        if (rb.position.x < worldCoords.worldLeft - 0.5f || rb.position.x > worldCoords.worldRight + 0.5f) {
            onEvacComplete(evacueeCount);
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        onKilled(evacueeCount);
        gameObject.SetActive(false);
    }
}
