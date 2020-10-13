using System;
using System.Collections;
using Shapes;
using UnityEngine;

public class City : MonoBehaviour {
    public bool isDestroyed = false;
    public Color evacuatedColor;
    public long population { get; private set; }
    private long evacuationRate;
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

    internal bool canEvacuate() {
        return !isDestroyed && population > 0;
    }

    public void initialise(long population, long evacuationRate) {
        this.population = population;
        this.evacuationRate = evacuationRate;
    }

    public long evacuate() {
        if (isDestroyed) {
            return 0;

        } else if (population <= evacuationRate) {
            var evacCount = population;
            population = 0;
            aliveVisuals.GetComponent<Polyline>().Color = evacuatedColor;
            return evacCount;

        } else {
            population -= evacuationRate;
            return evacuationRate;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!isDestroyed) {
            destroy();
        }
        other.gameObject.SendMessage("explode");
    }

    private void destroy() {
        if (isDestroyed) return;
        population = 0;
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
