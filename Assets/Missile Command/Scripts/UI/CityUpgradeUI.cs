using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityUpgradeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public UpgradeElement upgradeElementPrefab;
    public CityIconRegistry iconRegistry;
    public float firstUpgradeOffset;
    public float baseButtonOffset;
    public Collider2D expandedCollider;
    private Colors colors { get { return Colors.Instance; } }
    private City city;
    private Action onUpgradeAction;
    private Action onSelectionMadeAction;
    private List<UpgradeElement> elements;
    private Disc graphic;
    private bool isSelected;

    internal void initialise(City city, Action onSelectionMadeAction, Action onUpgradeAction) {
        isSelected = false;
        this.city = city;
        this.onSelectionMadeAction = onSelectionMadeAction;
        this.onUpgradeAction = onUpgradeAction;
        transform.position = city.transform.position + Vector3.up * baseButtonOffset;
        graphic = GetComponent<Disc>();
        graphic.Color = colors.upgradeUiNormalColor;
        expandedCollider.enabled = false;
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
        if (city == null) {
            Debug.Log("null city for enabled UpgradeUI; investigate");
            return;
        }
        if (elements == null) {
            elements = generateUpgradeElements();
        }
        foreach (var element in elements) {
            element.gameObject.SetActive(true);
        }
        elements[0].display(new UpgradeData(
            city.upgradeState.evacuatorCountUpgradeState(),
            iconRegistry.evacuatorCountIcon,
            () => {
                city.upgradeState.increaseEvacuatorCount();
                onUpgradeAction();
            }
        ));
        elements[1].display(new UpgradeData(
            city.upgradeState.evacuatorPopUpgradeState(),
            iconRegistry.popEvacRateIcon,
            () => {
                city.upgradeState.increaseEvacuatorPop();
                onUpgradeAction();
            }
        ));
    }

    private List<UpgradeElement> generateUpgradeElements() {
        var elements = new List<UpgradeElement>();
        elements.Add(createElement(0));
        elements.Add(createElement(1));
        return elements;
    }

    private UpgradeElement createElement(int index) {
        var element = GameObject.Instantiate<UpgradeElement>(upgradeElementPrefab, transform, false);
        element.setMeasurements(firstUpgradeOffset, GetComponentInChildren<Disc>().Radius, index);
        return element;
    }
}
