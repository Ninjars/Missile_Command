using UnityEngine;

public class City : MonoBehaviour {
    
    public bool isDestroyed = false;

    private ScreenEffectManager _screenEffectManager;
    private ScreenEffectManager screenEffectManager {
        get {
            if (_screenEffectManager == null) {
                _screenEffectManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<ScreenEffectManager>();
            }
            return _screenEffectManager;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        Debug.Log($"OnTriggerEnter2D {gameObject.name} -> {other.gameObject.name} on layer {LayerMask.LayerToName(other.gameObject.layer)}");
        if (!isDestroyed) {
            destroy();
        }
    }

    private void destroy() {
        isDestroyed = true;
        screenEffectManager.onCityNukeHit(1f);
    }
}
