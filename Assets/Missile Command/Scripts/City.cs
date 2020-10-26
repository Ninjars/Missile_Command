using System.Collections;
using Shapes;
using TMPro;
using UnityEngine;

public class City : MonoBehaviour {
    public float uiFadeDuration = 2f;
    public bool isDestroyed = false;
    public GameObject aliveVisuals;
    public GameObject deadVisuals;
    public GameObject textUi;
    public TextMeshProUGUI cityNameView;
    public TextMeshProUGUI cityPopulationView;
    public Explosion explosionPrefab;

    public long population { get; private set; }
    private StateUpdater stateUpdater;
    private Colors colors { get { return Colors.Instance; } }
    private Triangle markerTriangle;
    private CanvasGroup textCanvasGroup;
    private bool isFadingUi;
    private float fadeStart;
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

        cityNameView.text = gameObject.name;
        cityPopulationView.text = $"{population}";

        aliveVisuals.GetComponent<Polyline>().Color = colors.cityColor;
        markerTriangle = textUi.GetComponent<Triangle>();
        textCanvasGroup = textUi.GetComponentInChildren<CanvasGroup>();
        deadVisuals.GetComponent<Polyline>().Color = colors.deadBuildingColor;
    }

    public void showUi() {
        isFadingUi = false;
        textCanvasGroup.alpha = 1;
        Color color = isDestroyed ? colors.deadBuildingColor : colors.cityColor;
        markerTriangle.Color = color;
        cityNameView.color = color;
        cityPopulationView.color = color;
        cityPopulationView.text = isDestroyed ? "DEAD" : $"{population}";
        textUi.SetActive(true);

    }

    public void fadeOutUi() {
        if (!textUi.activeInHierarchy) return;
        isFadingUi = true;
        fadeStart = Time.time;
    }

    private void Update() {
        if (isFadingUi) {
            float fraction = (Time.time - fadeStart) / uiFadeDuration;
            if (fraction <= 1) {
                Color color = markerTriangle.Color;
                color.a = Mathf.Lerp(color.a, 0, fraction * fraction);
                markerTriangle.Color = color;
                textCanvasGroup.alpha = color.a;

            } else {
                textUi.SetActive(false);
                isFadingUi = false;
            }
        }
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

        if (isFadingUi) {
            showUi();
            fadeOutUi();
        }
    }

    private IEnumerator setDeadVisuals() {
        yield return new WaitForSeconds(0.2f);

        aliveVisuals.SetActive(false);
        deadVisuals.SetActive(true);
    }
}
