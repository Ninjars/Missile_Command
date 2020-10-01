using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class MissileExplosion : MonoBehaviour {
    public float maxRadius = 2;
    public float initialRadius = 0.1f;
    public float expandDuration = 0.75f;
    public float lingerDuration = 1f;
    public float layerZ = 6;

    private CircleCollider2D _circleCollider;
    private CircleCollider2D circleCollider {
        get {
            if (_circleCollider == null) {
                _circleCollider = GetComponent<CircleCollider2D>();
            }
            return _circleCollider;
        }
    }
    private Disc _discShape;
    private Disc discShape {
        get {
            if (_discShape == null) {
                _discShape = GetComponentInChildren<Disc>(true);
            }
            return _discShape;
        }
    }
    private float startTime;

    public void boom(Vector2 position) {
        transform.position = new Vector3(position.x, position.y, layerZ);
        discShape.Radius = initialRadius;
        circleCollider.radius = initialRadius;
        startTime = Time.time;
        gameObject.SetActive(true);
    }

    private void Update() {
        float passedTime = Time.time - startTime;
        if (passedTime <= expandDuration) {
            expandRadius(passedTime);
        } else if (passedTime > lingerDuration + expandDuration) {
            gameObject.SetActive(false);
        }
    }

    private void expandRadius(float passedTime) {
        float radius = Mathf.Lerp(initialRadius, maxRadius, Mathf.Clamp01(passedTime / expandDuration));
        discShape.Radius = radius;
        circleCollider.radius = radius;
    }
}
