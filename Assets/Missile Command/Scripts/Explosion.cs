using Shapes;
using UnityEngine;

public class Explosion : MonoBehaviour {
    public bool causesEmp = true;
    public float maxRadius = 2;
    public float duration = 2f;
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

    public void boom(Vector2 position, Color color) {
        transform.position = new Vector3(position.x, position.y, layerZ);
        discShape.Radius = 0;
        discShape.Color = color;
        circleCollider.radius = 0;
        startTime = Time.time;
        gameObject.SetActive(true);
        if (causesEmp) screenEffectManager.onEMP(position.y);
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
