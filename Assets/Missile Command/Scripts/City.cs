using System.Collections;
using Shapes;
using UnityEngine;

public class City : MonoBehaviour {
    public bool isDestroyed = false;
    public GameObject aliveVisuals;
    public GameObject deadVisuals;
    public Explosion explosionPrefab;

    public long population { get; private set; }
    private StateUpdater stateUpdater;
    private Colors colors { get { return Colors.Instance; }}

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

    public void initialise(StateUpdater stateUpdater, long population) {
        this.stateUpdater = stateUpdater;
        this.population = population;

        aliveVisuals.GetComponent<Polyline>().Color = colors.cityColor;
        deadVisuals.GetComponent<Polyline>().Color = colors.deadBuildingColor;
    }

    public long evacuate(long evacuationCountMax) {
        if (isDestroyed) {
            return 0;

        } else if (population <= evacuationCountMax) {
            var evacCount = population;
            population = 0;
            aliveVisuals.GetComponent<Polyline>().Color = colors.deadBuildingColor;
            return evacCount;

        } else {
            population -= evacuationCountMax;
            return evacuationCountMax;
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
        GetComponent<CircleCollider2D>().enabled = false;
        stateUpdater.onPopulationLost(population);
        population = 0;
        isDestroyed = true;
        screenEffectManager.onCityNukeHit(1f);

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(transform.position, colors.buildingExplodeColor);
        StartCoroutine(setDeadVisuals());
    }

    private IEnumerator setDeadVisuals() {
        yield return new WaitForSeconds(0.2f);

        aliveVisuals.SetActive(false);
        deadVisuals.SetActive(true);
    }
}
