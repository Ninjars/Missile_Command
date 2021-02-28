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
    private readonly int maxEvacuatorCountLevel = 3;
    private int evacuatorCountLevel = 0;
    public int evacuatorCount { get { return evacuatorCountLevel*2; } }
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
    private readonly int maxExplosionRadiusLevel = 3;
    private readonly float maxRadiusFactor = 1f;
    private int explosionRadiusLevel = 0;
    public float explosionRadiusFactor { get { return 1 + explosionRadiusLevel / (float) maxExplosionRadiusLevel * maxRadiusFactor; } }
    public void increaseExplosionRadius() {
        if (explosionRadiusLevel < maxExplosionRadiusLevel) {
            explosionRadiusLevel++;
        }
    }
    public UpgradeState explosionRadiusUpgradeState() {
        return new UpgradeState(maxExplosionRadiusLevel, explosionRadiusLevel);
    }

    private readonly int maxMissileSpeedLevel = 3;
    private readonly float maxMissileSpeedFactor = 1f;
    private int missileSpeedLevel = 0;
    public float missileSpeedFactor { get { return 1 + missileSpeedLevel / (float) maxMissileSpeedLevel * maxMissileSpeedFactor; } }
    public void increaseMissileSpeed() {
        if (missileSpeedLevel < maxMissileSpeedLevel) {
            missileSpeedLevel++;
        }
    }
    public UpgradeState missileSpeedUpgradeState() {
        return new UpgradeState(maxMissileSpeedLevel, missileSpeedLevel);
    }

    private readonly int maxExplosionLingerLevel = 3;
    private readonly float maxExplosionLingerfactor = 1.5f;
    private int explosionLingerLevel = 0;
    public float explosionLingerFactor { get { return 1 + explosionLingerLevel / (float) maxExplosionLingerLevel * maxExplosionLingerfactor; } }
    public void increaseExplosionLinger() {
        if (explosionLingerLevel < maxExplosionLingerLevel) {
            explosionLingerLevel++;
        }
    }
    public UpgradeState explosionLingerUpgradeState() {
        return new UpgradeState(maxExplosionLingerLevel, explosionLingerLevel);
    }
}