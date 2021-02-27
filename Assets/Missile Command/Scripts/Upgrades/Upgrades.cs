using System;
using UnityEngine;

public struct UpgradeData {
    public readonly UpgradeState state;
    public readonly GameObject icon;
    public readonly Action<UpgradeElement> upgradeAction;

    public UpgradeData(UpgradeState state, GameObject icon, Action<UpgradeElement> upgradeAction) {
        this.state = state;
        this.icon = icon;
        this.upgradeAction = upgradeAction;
    }
}

public struct UpgradeState {
    public readonly int maxLevel;
    public readonly int currentLevel;

    public UpgradeState(int maxLevel, int currentLevel) {
        this.maxLevel = maxLevel;
        this.currentLevel = currentLevel;
    }

    public bool canUpgrade { get { return currentLevel < maxLevel; } }
}

public class CityUpgradeState {
    public bool hasAnyAvailableUpgrades { get {
        return evacuatorCountLevel < maxEvacuatorCountLevel || evacuatorPopLevel < maxEvacuatorCountLevel;
    } }
    private readonly int maxEvacuatorCountLevel = 12;
    private int evacuatorCountLevel = 0;
    public int evacuatorCount { get { return evacuatorCountLevel; } }
    public void increaseEvacuatorCount() {
        if (evacuatorCountLevel < maxEvacuatorCountLevel) {
            evacuatorCountLevel++;
        }
    }
    public UpgradeState evacuatorCountUpgradeState() {
        return new UpgradeState(maxEvacuatorCountLevel, evacuatorCountLevel);
    }

    private readonly int maxEvacuatorPopLevel = 25;
    private int evacuatorPopLevel = 0;
    public double evacuatorPopFactor { get { return 1 + evacuatorPopLevel * 0.2; } }
    public void increaseEvacuatorPop() {
        if (evacuatorPopLevel < maxEvacuatorPopLevel) {
            evacuatorPopLevel++;
        }
    }
    public UpgradeState evacuatorPopUpgradeState() {
        return new UpgradeState(maxEvacuatorPopLevel, evacuatorPopLevel);
    }

    private readonly int maxShieldLevel = 3;
    public int shieldLevel { get; private set; }

    public void increaseShield() {
        if (shieldLevel < maxShieldLevel) {
            shieldLevel++;
        }
    }
    public void decreaseShield() {
        if (shieldLevel > 0) {
            shieldLevel--;
        }
    }
    public UpgradeState shieldUpgradeState() {
        return new UpgradeState(maxShieldLevel, shieldLevel);
    }
}

public class BatteryUpgradeState {
    public bool hasAnyAvailableUpgrades { get {
        return explosionRadiusLevel < maxExplosionRadiusLevel 
            || missileSpeedLevel < maxMissileSpeedLevel
            || explosionLingerLevel < maxExplosionLingerLevel;
    } }
    private readonly int maxExplosionRadiusLevel = 10;
    private int explosionRadiusLevel = 0;
    public float explosionRadiusFactor { get { return 1 + explosionRadiusLevel * 0.1f; } }
    public void increaseExplosionRadius() {
        if (explosionRadiusLevel < maxExplosionRadiusLevel) {
            explosionRadiusLevel++;
        }
    }
    public UpgradeState explosionRadiusUpgradeState() {
        return new UpgradeState(maxExplosionRadiusLevel, explosionRadiusLevel);
    }

    private readonly int maxMissileSpeedLevel = 10;
    private int missileSpeedLevel = 0;
    public float missileSpeedFactor { get { return 1 + missileSpeedLevel * 0.1f; } }
    public void increaseMissileSpeed() {
        if (missileSpeedLevel < maxMissileSpeedLevel) {
            missileSpeedLevel++;
        }
    }
    public UpgradeState missileSpeedUpgradeState() {
        return new UpgradeState(maxMissileSpeedLevel, missileSpeedLevel);
    }

    private readonly int maxExplosionLingerLevel = 10;
    private int explosionLingerLevel = 0;
    public float explosionLingerFactor { get { return 1 + explosionLingerLevel * 0.1f; } }
    public void increaseExplosionLinger() {
        if (explosionLingerLevel < maxExplosionLingerLevel) {
            explosionLingerLevel++;
        }
    }
    public UpgradeState explosionLingerUpgradeState() {
        return new UpgradeState(maxExplosionLingerLevel, explosionLingerLevel);
    }
}