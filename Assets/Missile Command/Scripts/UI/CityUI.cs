using Shapes;
using TMPro;
using UnityEngine;

public class CityUI : MonoBehaviour {
    public float uiFadeDuration = 2f;
    public float animationDuration = 1.66f;
    private bool isAnimating;
    public GameObject textUi;
    public TextMeshProUGUI cityNameView;
    public TextMeshProUGUI cityPopulationView;
    public Triangle markerTriangle;

    private Colors colors { get { return Colors.Instance; } }
    private bool animateCityUi;
    private float animStart;
    private CanvasGroup textCanvasGroup;
    private bool isDead;
    private bool isFadingUi;
    private float fadeStart;

    internal void initialise(string name, long population) {
        cityNameView.text = name;
        cityPopulationView.text = $"{population}";
        textCanvasGroup = textUi.GetComponentInChildren<CanvasGroup>();
    }

    private void Update() {
        if (isAnimating) {
            float animPercentage = ((Time.time - animStart) % animationDuration) / animationDuration;
            float swivelRadians = animPercentage * Mathf.PI * 2;
            float swivelValue = Mathf.Sin(swivelRadians);
            float bounceRadians = animPercentage * Mathf.PI * 4;
            float bounceValue = Mathf.Sin(swivelRadians);

            markerTriangle.transform.localScale = new Vector3(swivelValue, 1, 1);
            markerTriangle.transform.localPosition = new Vector3(0, bounceValue * 0.05f, 0);
        }
        if (isFadingUi) {
            float fraction = (Time.time - fadeStart) / uiFadeDuration;
            if (fraction <= 1) {
                fade(fraction);

            } else {
                textUi.SetActive(false);
                isFadingUi = false;
                stopAnimating();
            }
        }
    }

    private void fade(float fraction) {
        Color color = markerTriangle.Color;
        float alpha = Mathf.Lerp(color.a, 0, fraction * fraction);
        color.a = alpha;
        markerTriangle.Color = color;
        textCanvasGroup.alpha = alpha;
    }

    private void startAnimating() {
        animStart = Time.time;
        markerTriangle.Color = colors.upgradeUiHighlightedColor;
        textCanvasGroup.alpha = 1;
        isAnimating = true;
    }

    private void stopAnimating() {
        if (isAnimating) {
            Color color = isDead ? colors.deadBuildingColor : colors.textColor;
            markerTriangle.Color = color;
            markerTriangle.transform.localScale = Vector3.one;
            markerTriangle.transform.localPosition = Vector3.zero;
            isAnimating = false;
        }
    }

    private void showElements() {
        Color color = isDead ? colors.deadBuildingColor : colors.textColor;
        markerTriangle.Color = color;
        isAnimating = false;

        textCanvasGroup.alpha = 1;
        cityNameView.color = color;
        cityPopulationView.color = color;
        textUi.SetActive(true);
    }

    internal void display(bool upgrading) {
        isFadingUi = false;
        isAnimating = false;
        showElements();
        if (!isDead && upgrading) {
            startAnimating();
        }
    }

    internal void onHighlight(bool isHighlighted) {
        if (isHighlighted) {
            isAnimating = false;
            showElements();
            markerTriangle.Color = colors.upgradeUiHighlightedColor;
            markerTriangle.transform.localScale = Vector3.one;
            markerTriangle.transform.localPosition = Vector3.zero;
        } else {
            showElements();
            isAnimating = true;
            markerTriangle.Color = colors.upgradeUiHighlightedColor;
            markerTriangle.transform.localScale = Vector3.one;
        }
    }

    internal void onHide(bool fade) {
        isFadingUi = false;
        if (!fade) { 
            textUi.SetActive(false);
        } else {
            if (!textUi.activeInHierarchy) return;
            isFadingUi = true;
            fadeStart = Time.time;
        }
    }

    internal void onDead() {
        isDead = true;
        if (!isAnimating) {
            display(false);
            onHide(true);
        } else {
            stopAnimating();
        }
    }

    internal void updateCityPopReadoutContent(long population) {
        if (cityPopulationView.isActiveAndEnabled) {
            cityPopulationView.text = isDead ? "DEAD" : $"{population}";
        }
    }
}
