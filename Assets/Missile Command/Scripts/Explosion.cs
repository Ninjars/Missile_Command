using Shapes;
using UnityEngine;

public class Explosion : MonoBehaviour {
    public bool causesEmp = true;
    public float defaultRadius = 1;
    public float defaultDuration = 1f;
    public AnimationCurve expansion;
    public float layerZ = 6;
    private ScreenEffectManager _screenEffectManager;
    private ScreenEffectManager screenEffectManager {
        get {
            if (_screenEffectManager == null) {
                _screenEffectManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScreenEffectManager>();
            }
            return _screenEffectManager;
        }
    }

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
    private float radius;
    private float duration;

    public void boom(Vector2 position, Color color) {
        boom(position, color, defaultRadius, defaultDuration);
    }

    public void boom(
        Vector2 position, 
        Color color, 
        float explosionRadius, 
        float explosionDuration
    ) {
        transform.position = new Vector3(position.x, position.y, layerZ);
        discShape.Radius = 0;
        discShape.Color = color;
        circleCollider.radius = 0;
        startTime = Time.time;
        radius = explosionRadius;
        duration = explosionDuration;
        gameObject.SetActive(true);
        if (causesEmp) screenEffectManager.onEMP(explosionRadius, position.y);
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
        float currentRadius = expansion.Evaluate(normalisedValue) * radius;
        discShape.Radius = currentRadius;
        circleCollider.radius = currentRadius;
    }
}
