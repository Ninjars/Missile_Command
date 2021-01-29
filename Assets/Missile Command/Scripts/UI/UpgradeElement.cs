using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using UnityEngine.EventSystems;

public class UpgradeElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    public float elementSeparation = 0.5f;
    private Colors colors { get { return Colors.Instance; } }
    private Line line;
    private Disc disc;
    private UpgradeData upgradeData;
    private GameObject icon;

    private void Awake() {
        line = GetComponentInChildren<Line>();
        disc = GetComponentInChildren<Disc>();
    }

    internal void display(UpgradeData upgradeData) {
        this.upgradeData = upgradeData;
        setNormalColor();
        updateIcon(upgradeData.icon);
    }

    private void updateIcon(GameObject newIcon) {
        if (icon == null) {
            icon = GameObject.Instantiate(newIcon, transform, false);

        } else if (icon.gameObject.name != upgradeData.icon.name) {
            GameObject.Destroy(icon);
            icon = GameObject.Instantiate(newIcon, transform, false);
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (upgradeData.state.canUpgrade) {
            upgradeData.upgradeAction();
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (upgradeData.state.canUpgrade) {
            disc.Color = colors.upgradeUiHighlightedColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        setNormalColor();
    }

    private void setNormalColor() {
        if (upgradeData.state.canUpgrade) {
            disc.Color = colors.upgradeUiNormalColor;
        } else {
            disc.Color = colors.upgradeUiMaxedColor;
        }
    }

    internal void setMeasurements(float baseElementOffset, float baseElementRadius, int index) {
        var radius = disc.Radius;
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
