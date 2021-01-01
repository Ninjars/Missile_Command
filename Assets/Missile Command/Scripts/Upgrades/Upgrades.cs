using System.Collections;
using System.Collections.Generic;

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
    private readonly int maxEvacuatorCountLevel = 13;
    private int evacuatorCountLevel = 0;
    public int evacuatorCount { get { return 3 + evacuatorCountLevel; } }
    public void increaseEvacuatorCount() {
        if (evacuatorCountLevel < maxEvacuatorCountLevel) {
            evacuatorCountLevel++;
        }
    }
    public UpgradeState evacuatorCountUpgradeState() {
        return new UpgradeState(maxEvacuatorCountLevel, evacuatorPopLevel);
    }

    private readonly int maxEvacuatorPopLevel = 20;
    private int evacuatorPopLevel = 0;
    public float evacuatorPopFactor { get { return 1 + evacuatorPopLevel * 0.2f; } }
    public void increaseEvacuatorPop() {
        if (evacuatorPopLevel < maxEvacuatorPopLevel) {
            evacuatorPopLevel++;
        }
    }
    public UpgradeState evacuatorPopUpgradeState() {
        return new UpgradeState(maxEvacuatorPopLevel, evacuatorPopLevel);
    }

}

public class BatteryUpgadeState {
    private readonly int maxExplosionRadiusLevel = 10;
    private int explosionRadiusLevel = 0;
    public float explosionRadiusFactor { get { return 1 + explosionRadiusLevel * 0.2f; } }
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