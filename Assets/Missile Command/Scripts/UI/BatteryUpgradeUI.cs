﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BatteryUpgradeUI : BaseUpgradeUi {
    public BatteryIconRegistry iconRegistry;
    private MissileBattery battery;
    private BatteryUI batteryUI;

    private void Awake() {
        onAwake();
        battery = GetComponentInParent<MissileBattery>();
        batteryUI = GetComponentInParent<BatteryUI>();
    }

    public override void onSelect() {
        base.onSelect();
        batteryUI.onHighlight(true);
    }

    public override void onDeselect() {
        base.onDeselect();
        batteryUI.onHighlight(false);
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
                (element) => {
                    battery.upgradeState.increaseMissileSpeed();
                    onUpgradeAction(element);
                }
            ),
            new UpgradeData(
                battery.upgradeState.explosionRadiusUpgradeState(),
                iconRegistry.explosionRadiusIcon,
                (element) => {
                    battery.upgradeState.increaseExplosionRadius();
                    onUpgradeAction(element);
                }
            ),
            new UpgradeData(
                battery.upgradeState.explosionLingerUpgradeState(),
                iconRegistry.explosionDurationIcon,
                (element) => {
                    battery.upgradeState.increaseExplosionLinger();
                    onUpgradeAction(element);
                }
            )
        }.Where(data => data.state.canUpgrade).ToList();
    }
}
