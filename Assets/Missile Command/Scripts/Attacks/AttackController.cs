using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackController : MonoBehaviour {
    private List<Coroutine> currentAttacks;
    private AttackUtil attackUtil = new AttackUtil();
    public int pendingAttackCount;

    internal void scheduleAttackEvents(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        List<City> cities,
        List<MissileBattery> missileBatteries,
        List<WeaponData> weaponDatas,
        float stageDuration,
        float stageProgress
    ) {
        clearCurrentAttacks();

        foreach (var weaponData in weaponDatas) {
            int attackCount = weaponData.count.evaluate(stageProgress);
            pendingAttackCount += attackCount;
            float baseInterval = stageDuration / attackCount;
            for (int i = 0; i < attackCount; i++) {
                float timeToAttack = weaponData.initialDelay + i * baseInterval + getAttackVariance(stageProgress, baseInterval, weaponData.intervalVariance);
                timeToAttack = Mathf.Min(timeToAttack, stageDuration + baseInterval);
                IEnumerator attack = null;
                if (weaponData is ICBMData) {
                    attack = attackUtil.scheduleIcbmAttack(
                        stateUpdater,
                        worldCoords,
                        timeToAttack,
                        (ICBMData)weaponData,
                        stageProgress,
                        () => { pendingAttackCount--; },
                        () => getTargetPosition(worldCoords, weaponData.targetWeights, cities, missileBatteries)
                    );
                } else if (weaponData is BomberData) {
                    attack = attackUtil.scheduleBomberAttack(
                        stateUpdater,
                        worldCoords,
                        timeToAttack,
                        (BomberData)weaponData,
                        stageProgress,
                        () => { pendingAttackCount--; },
                        (currentPositionAndVector) => getBomberTarget(
                            worldCoords,
                            currentPositionAndVector,
                            ((BomberData)weaponData).bombAttackData.targetWeights,
                            cities,
                            missileBatteries
                        )
                    );
                }

                if (attack == null) {
                    throw new InvalidOperationException($"Unhandled WeaponData type: {weaponData}");
                } else {
                    currentAttacks.Add(StartCoroutine(attack));
                }
            }
        }
    }

    internal void stopAttacks() {
        clearCurrentAttacks();
    }

    private void clearCurrentAttacks() {
        pendingAttackCount = 0;
        if (currentAttacks == null) {
            currentAttacks = new List<Coroutine>();
            return;
        }
        foreach (var coroutine in currentAttacks) {
            StopCoroutine(coroutine);
        }
        currentAttacks.Clear();
    }

    private static float getAttackVariance(float stageProgress, float baseInterval, RangeData intervalVariance) {
        float maxVariance = intervalVariance.evaluate(stageProgress) * baseInterval;
        float variance = maxVariance * UnityEngine.Random.value * 2 - maxVariance;
        return variance;
    }

    private Vector2 getTargetPosition(
        WorldCoords worldCoords,
        TargetWeights targetWeights,
        List<City> cities,
        List<MissileBattery> missileBatteries
    ) {
        TargetType targetType = targetWeights.getTargetType(UnityEngine.Random.value);
        switch (targetType) {
            case TargetType.CITY: {
                    City city = getTargetableCity(cities);
                    if (city == null) {
                        return getRandomTargetPoint(worldCoords);
                    } else {
                        return city.transform.position;
                    }
                }
            case TargetType.BATTERY: {
                    MissileBattery battery = getTargetableBattery(missileBatteries);
                    if (battery == null) {
                        return getRandomTargetPoint(worldCoords);
                    } else {
                        return battery.transform.position;
                    }
                }
            case TargetType.RANDOM: {
                    return getRandomTargetPoint(worldCoords);
                }
            default: {
                    throw new InvalidOperationException($"unhandled case {targetType}");
                }
        }
    }

    private City getTargetableCity(List<City> cities) {
        return cities
                .Where(it => !it.isDestroyed)
                .OrderBy(n => UnityEngine.Random.value)
                .FirstOrDefault();
    }

    private MissileBattery getTargetableBattery(List<MissileBattery> batteries) {
        return batteries
                .Where(it => !it.isDestroyed)
                .OrderBy(n => UnityEngine.Random.value)
                .FirstOrDefault();
    }

    private Vector2 getRandomTargetPoint(WorldCoords worldCoords) {
        return new Vector2(
            UnityEngine.Random.value * worldCoords.width + worldCoords.worldLeft,
            worldCoords.groundY
        );
    }

    private Vector2 getBomberTarget(
        WorldCoords worldCoords,
        Vector3 currentPositionAndVector,
        TargetWeights targetWeights,
        List<City> cities,
        List<MissileBattery> missileBatteries
    ) {
        TargetType targetType = targetWeights.getTargetType(UnityEngine.Random.value);
        float currentX = currentPositionAndVector.x;
        bool isTravellingRight = currentPositionAndVector.z > 0;
        switch (targetType) {
            case TargetType.CITY: {
                    City city = getNextCity(cities, currentX, isTravellingRight);
                    if (city == null) {
                        return getRandomBomberTarget(worldCoords, currentX, isTravellingRight);
                    } else {
                        return city.transform.position;
                    }
                }
            case TargetType.BATTERY: {
                    MissileBattery battery = getNextBattery(missileBatteries, currentX, isTravellingRight);
                    if (battery == null) {
                        return getRandomBomberTarget(worldCoords, currentX, isTravellingRight);
                    } else {
                        return battery.transform.position;
                    }
                }
            case TargetType.RANDOM: {
                    return getRandomBomberTarget(worldCoords, currentX, isTravellingRight);
                }
            default: {
                    throw new InvalidOperationException($"unhandled case {targetType}");
                }
        }
    }

    internal bool allAttacksLaunched() {
        return pendingAttackCount == 0;
    }

    private City getNextCity(List<City> cities, float currentX, bool travellingRight) {
        City best = null;
        foreach (var city in cities) {
            if (travellingRight) {
                if (city.transform.position.x > currentX && !city.isDestroyed) {
                    if (best == null) {
                        best = city;
                    } else if (city.transform.position.x < best.transform.position.x) {
                        best = city;
                    } else {
                        return best;
                    }
                }
            } else {
                if (city.transform.position.x < currentX && !city.isDestroyed) {
                    if (best == null) {
                        best = city;
                    } else if (city.transform.position.x > best.transform.position.x) {
                        best = city;
                    } else {
                        return best;
                    }
                }
            }
        }
        return best;
    }

    private MissileBattery getNextBattery(List<MissileBattery> batteries, float currentX, bool travellingRight) {
        MissileBattery best = null;
        foreach (var battery in batteries) {
            if (travellingRight) {
                if (battery.transform.position.x > currentX && !battery.isDestroyed) {
                    if (best == null) {
                        best = battery;
                    } else if (battery.transform.position.x < best.transform.position.x) {
                        best = battery;
                    } else {
                        return best;
                    }
                }
            } else if (battery.transform.position.x < currentX && !battery.isDestroyed) {
                    if (best == null) {
                        best = battery;
                    } else if (battery.transform.position.x > best.transform.position.x) {
                        best = battery;
                    } else {
                        return best;
                    }
            }
        }
        return best;
    }

    private Vector2 getRandomBomberTarget(WorldCoords worldCoords, float currentX, bool travellingRight) {
        if (travellingRight) {
            return new Vector2(
                currentX + UnityEngine.Random.value * (worldCoords.worldRight - currentX),
                worldCoords.groundY
            );
        } else {
            return new Vector2(
                currentX - UnityEngine.Random.value * (currentX - worldCoords.worldLeft),
                worldCoords.groundY
            );
        }
    }
}

[Serializable]
public struct TargetWeights {
    public float city, battery, random;

    private float sum { get { return city + battery + random; } }

    public TargetType getTargetType(float value) {
        if (sum == 0) return TargetType.RANDOM;

        if (value <= city / sum) {
            return TargetType.CITY;
        } else if (value <= battery / sum) {
            return TargetType.BATTERY;
        } else {
            return TargetType.RANDOM;
        }
    }
}

public enum TargetType {
    CITY, BATTERY, RANDOM,
}
