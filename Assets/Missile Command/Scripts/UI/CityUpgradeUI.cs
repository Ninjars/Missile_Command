using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityUpgradeUI : BaseUpgradeUi {
    public CityIconRegistry iconRegistry;
    private City city;

    private void Awake() {
        onAwake();
        city = GetComponentInParent<City>();
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
}
