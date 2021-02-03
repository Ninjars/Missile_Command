using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityUpgradeUI : BaseUpgradeUi {
    public CityIconRegistry iconRegistry;
    private City city;
    private CityUI cityUI;

    private void Awake() {
        onAwake();
        city = GetComponentInParent<City>();
        cityUI = GetComponentInParent<CityUI>();
    }

    public override void onSelect() {
        base.onSelect();
        cityUI.onHighlight(true);
    }

    public override void onDeselect() {
        base.onDeselect();
        cityUI.onHighlight(false);
    }

    override protected List<UpgradeData> constructUpgradeData() {
        if (city == null) {
            Debug.Log("null city for enabled constructUpgradeData; investigate");
            return new List<UpgradeData>();
        }

        return new List<UpgradeData>{
            new UpgradeData(
                city.upgradeState.evacuatorCountUpgradeState(),
                iconRegistry.evacuatorCountIcon,
                (element) => {
                    city.upgradeState.increaseEvacuatorCount();
                    onUpgradeAction(element);
                }
            ),
            new UpgradeData(
                city.upgradeState.evacuatorPopUpgradeState(),
                iconRegistry.popEvacRateIcon,
                (element) => {
                    city.upgradeState.increaseEvacuatorPop();
                    onUpgradeAction(element);
                }
            )
        }.Where(data => data.state.canUpgrade).ToList();
    }
}
