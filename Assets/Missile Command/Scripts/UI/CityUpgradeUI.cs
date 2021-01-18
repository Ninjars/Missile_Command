using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class CityUpgradeUI : MonoBehaviour {
    public UpgradeElement upgradeElementPrefab;
    public float baseElementOffset;

    private City city;
    private Action onUpgradeAction;
    private List<UpgradeElement> elements;

    internal void initialise(City city, Action onUpgradeAction) {
        this.city = city;
        this.onUpgradeAction = onUpgradeAction;
        transform.position = city.transform.position;
    }

    private void OnEnable() {
        if (city == null) {
            Debug.Log("null city for enabled UpgradeUI; investigate");
            return;
        }
        if (elements == null) {
            elements = generateUpgradeElements();
        }
        elements[0].display(city.upgradeState.evacuatorCountUpgradeState(), () => {
            city.upgradeState.increaseEvacuatorCount();
            onUpgradeAction();
        });
        elements[1].display(city.upgradeState.evacuatorPopUpgradeState(), () => {
            city.upgradeState.increaseEvacuatorPop();
            onUpgradeAction();
        });
    }

    private List<UpgradeElement> generateUpgradeElements() {
        var elements = new List<UpgradeElement>();
        elements.Add(createElement(0));
        elements.Add(createElement(1));
        return elements;
    }

    private UpgradeElement createElement(int index) {
        var element = GameObject.Instantiate<UpgradeElement>(upgradeElementPrefab, transform, false);
        element.setMeasurements(baseElementOffset, GetComponentInChildren<Disc>().Radius, index);
        return element;
    }
}
