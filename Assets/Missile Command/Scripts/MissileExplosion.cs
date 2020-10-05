using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class MissileExplosion : MonoBehaviour {
    public float maxRadius = 2;
    public float duration = 2f;
    public AnimationCurve expansion;
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
        discShape.Radius = 0;
        circleCollider.radius = 0;
        startTime = Time.time;
        gameObject.SetActive(true);
    }

    private void Update() {
        float passedTime = Time.time - startTime;
        if (passedTime > duration) {
            gameObject.SetActive(false);
        } else {
            expandRadius(passedTime / duration);
        }
    }

    private void expandRadius(float normalisedValue) {
        float radius = expansion.Evaluate(normalisedValue) * maxRadius;
        discShape.Radius = radius;
        circleCollider.radius = radius;
    }
}
