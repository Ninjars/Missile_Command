using System;
using System.Collections.Generic;
using System.Linq;
using Shapes;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityUpgradeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public UpgradeElement upgradeElementPrefab;
    public CityIconRegistry iconRegistry;
    public float firstUpgradeOffset;
    public Collider2D expandedCollider;
    private Colors colors { get { return Colors.Instance; } }
    private City city;
    private Action onUpgradeAction;
    private Action onSelectionMadeAction;
    private List<UpgradeElement> elements;
    private Disc graphic;
    private bool isSelected;
    private int availableUpgradeCount;

    private void Awake() {
        isSelected = false;
        city = GetComponentInParent<City>();
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

    private void OnDisable() {
        hideUpgrades();
    }

    private void displayUpgrades() {
        if (city == null) {
            Debug.Log("null city for enabled UpgradeUI; investigate");
            return;
        }

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

    private List<UpgradeData> constructUpgradeData() {
        return new List<UpgradeData>{
            new UpgradeData(
                city.upgradeState.evacuatorCountUpgradeState(),
                iconRegistry.evacuatorCountIcon,
                () => {
                    city.upgradeState.increaseEvacuatorCount();
                    onUpgradeAction();
                }
            ),
            new UpgradeData(
                city.upgradeState.evacuatorPopUpgradeState(),
                iconRegistry.popEvacRateIcon,
                () => {
                    city.upgradeState.increaseEvacuatorPop();
                    onUpgradeAction();
                }
            )
        }.Where(data => data.state.canUpgrade).ToList();
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

    internal void registerCallbacks(Action onHighlightCallback, Action onUpgradeCallback) {
        onSelectionMadeAction = onHighlightCallback;
        onUpgradeAction = onUpgradeCallback;
    }
}
