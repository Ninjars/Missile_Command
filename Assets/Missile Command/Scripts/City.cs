using System.Collections;
using UnityEngine;

public class City : MonoBehaviour {
    
    public bool isDestroyed = false;
    public GameObject aliveVisuals;
    public GameObject deadVisuals;
    public Explosion explosionPrefab;

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
        if (!isDestroyed) {
            destroy();
        }
    }

    private void destroy() {
        if (isDestroyed) return;
        
        isDestroyed = true;
        screenEffectManager.onCityNukeHit(1f);
        
        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(transform.position);
        StartCoroutine(setDeadVisuals());
    }

    private IEnumerator setDeadVisuals() {
        yield return new WaitForSeconds(0.2f);

        aliveVisuals.SetActive(false);
        deadVisuals.SetActive(true);
    }
}
