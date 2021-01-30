using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseUpgradeUi : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler {
    public UpgradeElement upgradeElementPrefab;
    public float firstUpgradeOffset;
    public Collider2D expandedCollider;
    private Colors colors { get { return Colors.Instance; } }
    private List<UpgradeElement> elements;
    private Disc graphic;
    private bool isSelected;
    private int availableUpgradeCount;
    protected Action onUpgradeAction;
    protected Action onSelectionMadeAction;

    protected void onAwake() {
        isSelected = false;
        graphic = GetComponent<Disc>();
        graphic.Color = colors.upgradeUiNormalColor;
        expandedCollider.enabled = false;
    }

    private void OnDisable() {
        hideUpgrades();
    }

    internal void registerCallbacks(Action onHighlightCallback, Action onUpgradeCallback) {
        onSelectionMadeAction = onHighlightCallback;
        onUpgradeAction = onUpgradeCallback;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        onSelect();
    }

    public void OnPointerExit(PointerEventData eventData) {
        onDeselect();
    }

    private void onSelect() {
        onSelectionMadeAction();
        isSelected = true;
        displayUpgrades();
        graphic.Color = colors.upgradeUiHighlightedColor;
        expandedCollider.enabled = true;
    }

    public void onDeselect() {
        isSelected = false;
        hideUpgrades();
        graphic.Color = colors.upgradeUiNormalColor;
        expandedCollider.enabled = false;
    }

    private void hideUpgrades() {
        if (elements != null) {
            foreach (var element in elements) {
                element.gameObject.SetActive(false);
            }
        }
    }

    private void displayUpgrades() {
        List<UpgradeData> upgradeData = constructUpgradeData();
        if (elements != null && upgradeData.Count != availableUpgradeCount) {
            foreach (var element in elements) {
                GameObject.Destroy(element.gameObject);
            }
            elements = null;
        }
        availableUpgradeCount = upgradeData.Count;

        if (elements == null) {
            elements = generateUpgradeElements(availableUpgradeCount);
        }
        for (int i = 0; i < availableUpgradeCount; i++) {
            elements[i].gameObject.SetActive(true);
            elements[i].display(upgradeData[i]);
        }
    }

    private List<UpgradeElement> generateUpgradeElements(int count) {
        var elements = new List<UpgradeElement>(count);
        for (int i = 0; i < count; i++) {
            elements.Add(createElement(i));
        }
        return elements;
    }

    private UpgradeElement createElement(int index) {
        var element = GameObject.Instantiate<UpgradeElement>(upgradeElementPrefab, transform, false);
        element.setMeasurements(firstUpgradeOffset, GetComponentInChildren<Disc>().Radius, index);
        return element;
    }

    abstract protected List<UpgradeData> constructUpgradeData();
    
}
