using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using UnityEngine.EventSystems;

public class UpgradeElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public float elementSeparation = 0.5f;
    public Disc outline;
    public Disc progressBar;
    public AudioClip mouseOverSound;
    public AudioClip selectSound;
    private Colors colors { get { return Colors.Instance; } }
    private Line line;
    private UpgradeData upgradeData;
    private GameObject icon;

    private void Awake() {
        line = GetComponentInChildren<Line>();
        setColors();
    }

    private void OnDisable() {
        setColors();
    }

    internal void display(UpgradeData upgradeData) {
        this.upgradeData = upgradeData;
        updateIcon(upgradeData.icon);
        updateProgress(upgradeData.state.currentLevel / (float) upgradeData.state.maxLevel);
    }

    private void updateIcon(GameObject newIcon) {
        if (icon == null) {
            icon = GameObject.Instantiate(newIcon, transform, false);

        } else if (icon.gameObject.name != upgradeData.icon.name) {
            GameObject.Destroy(icon);
            icon = GameObject.Instantiate(newIcon, transform, false);
        }
    }

    private void updateProgress(float fractionComplete) {
        progressBar.AngRadiansEnd = Mathf.PI / 2f - (2 * Mathf.PI * fractionComplete);
    }

    public void OnPointerClick(PointerEventData eventData) {
        upgradeData.upgradeAction(this);
        SoundManager.Instance.play(selectSound, volume:0.5f);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        outline.Color = colors.upgradeUiHighlightedColor;
        SoundManager.Instance.play(mouseOverSound, volume:0.5f);
    }

    public void OnPointerExit(PointerEventData eventData) {
        outline.Color = colors.upgradeUiNormalColor;
    }

    private void setColors() {
        line.Color = colors.upgradeUiNormalColor;
        outline.Color = colors.upgradeUiNormalColor;
        progressBar.Color = colors.upgradeUiProgressColor;
    }

    internal void setMeasurements(float baseElementOffset, float baseElementRadius, int index) {
        var radius = outline.Radius;
        transform.localPosition = Vector3.up * (baseElementOffset + (elementSeparation + 2 * radius) * index) + Vector3.forward * -1;
        if (index == 0) {
            line.gameObject.SetActive(false);
        } else {
            line.gameObject.SetActive(true);
            line.Start = new Vector3(0, -radius, 0);
            var lineLength = index == 0 ? baseElementOffset - baseElementRadius - radius : elementSeparation;
            line.End = new Vector3(0, line.Start.y - lineLength, 0);
        }
    }
}
