using System.Collections;
using System.Collections.Generic;
using Shapes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour {
    public float uiFadeDuration = 2f;
    public float animationDuration = 1.66f;
    public GameObject labelRootObject;
    public Triangle markerTriangle;

    private Colors colors { get { return Colors.Instance; } }
    private TextMeshProUGUI labelText;
    private Image labelBackground;
    private bool isAnimating;
    private bool isDead;
    private float animStart;

    private void Awake() {
        labelText = labelRootObject.GetComponentInChildren<TextMeshProUGUI>();
        labelBackground = labelRootObject.GetComponentInChildren<Image>();
        labelRootObject.SetActive(false);
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
    }


    private void startAnimating() {
        animStart = Time.time;
        markerTriangle.Color = colors.upgradeUiHighlightedColor;
        labelText.alpha = 1;
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

    internal void displayLabel() {
        isAnimating = false;
        if (!isDead) {
            labelRootObject.SetActive(true);
            labelText.text = gameObject.name;
            labelText.color = colors.labelTextColor;
            labelBackground.color = colors.labelBackgroundColor;
            markerTriangle.Color = colors.batteryColor;
            markerTriangle.transform.localScale = Vector3.one;
            markerTriangle.transform.localPosition = Vector3.zero;
        }
    }
    
    internal void displayIndicator() {
        if (!isDead) {
            showIndicator();
            isAnimating = true;
            startAnimating();
        }
    }

    private void showIndicator() {
        markerTriangle.Color = colors.upgradeUiHighlightedColor;
        labelRootObject.SetActive(false);
    }

    internal void onHighlight(bool isHighlighted) {
        if (isHighlighted) {
            showIndicator();
            isAnimating = false;
            markerTriangle.Color = colors.upgradeUiHighlightedColor;
            markerTriangle.transform.localScale = Vector3.one;
            markerTriangle.transform.localPosition = Vector3.zero;
        } else {
            showIndicator();
            isAnimating = true;
            markerTriangle.Color = colors.upgradeUiHighlightedColor;
            markerTriangle.transform.localScale = Vector3.one;
        }
    }

    internal void onHide() {
        labelRootObject.SetActive(false);
        markerTriangle.gameObject.SetActive(false);
        stopAnimating();
    }

    internal void onDead() {
        isDead = true;
        onHide();
    }
}
