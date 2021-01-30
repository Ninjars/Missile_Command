using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatteryUpgradeUI : BaseUpgradeUi {
    public BatteryIconRegistry iconRegistry;
    private MissileBattery battery;

    private void Awake() {
        onAwake();
        battery = GetComponentInParent<MissileBattery>();
    }

    override protected List<UpgradeData> constructUpgradeData() {
        if (battery == null) {
            Debug.Log("null battery for enabled constructUpgradeData; investigate");
            return new List<UpgradeData>();
        }

        return new List<UpgradeData>{
            new UpgradeData(
                battery.upgradeState.missileSpeedUpgradeState(),
                iconRegistry.missileSpeedIcon,
                () => {
                    battery.upgradeState.increaseMissileSpeed();
                    onUpgradeAction();
                }
            ),
            new UpgradeData(
                battery.upgradeState.explosionRadiusUpgradeState(),
                iconRegistry.explosionRadiusIcon,
                () => {
                    battery.upgradeState.increaseExplosionRadius();
                    onUpgradeAction();
                }
            ),
            new UpgradeData(
                battery.upgradeState.explosionLingerUpgradeState(),
                iconRegistry.explosionDurationIcon,
                () => {
                    battery.upgradeState.increaseExplosionLinger();
                    onUpgradeAction();
                }
            )
        }.Where(data => data.state.canUpgrade).ToList();
    }
}
