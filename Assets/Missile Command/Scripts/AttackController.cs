using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackController : MonoBehaviour {
    private List<Coroutine> currentAttacks;

    internal void scheduleAttackEvents(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        List<City> cities,
        List<MissileBattery> missileBatteries,
        ICBMData icbmData,
        float stageProgress
    ) {
        clearCurrentAttacks();

        float accumulatedTime = 0;
        int attackCount = icbmData.count.evaluate(stageProgress);
        for (int i = 0; i < attackCount; i++) {
            float attackInterval;
            if (i == 0) {
                attackInterval = icbmData.initialDelay;
            } else {
                attackInterval = getAttackInterval(stageProgress, icbmData.avgInterval, icbmData.intervalVariance);
            }
            accumulatedTime += attackInterval;
            currentAttacks.Add(StartCoroutine(
                scheduleAttack(
                    stateUpdater,
                    worldCoords,
                    cities,
                    missileBatteries,
                    accumulatedTime,
                    icbmData,
                    stageProgress
                )
            ));
        }
        stateUpdater.setLevelEnd(Time.time + accumulatedTime);
    }

    internal void stopAttacks() {
        clearCurrentAttacks();
    }

    private void clearCurrentAttacks() {
        if (currentAttacks == null) {
            currentAttacks = new List<Coroutine>();
            return;
        }
        foreach (var coroutine in currentAttacks) {
            StopCoroutine(coroutine);
        }
        currentAttacks.Clear();
    }

    private float getAttackInterval(float stageProgress, RangeData avgInterval, RangeData intervalVariance) {
        float baseInterval = avgInterval.evaluate(stageProgress);
        float maxStageIntervalVariance = intervalVariance.evaluate(stageProgress);
        float variance = maxStageIntervalVariance * UnityEngine.Random.value * 2 - maxStageIntervalVariance;
        return baseInterval + variance;
    }

    private IEnumerator scheduleAttack(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        List<City> cities,
        List<MissileBattery> missileBatteries,
        float delay,
        ICBMData icbmData,
        float stageProgress
    ) {
        yield return new WaitForSeconds(delay);

        ICBM weapon = ObjectPoolManager.Instance.getObjectInstance(icbmData.weaponPrefab.gameObject).GetComponent<ICBM>();
        
        weapon.launch(
            stateUpdater,
            worldCoords,
            icbmData,
            stageProgress,
            () => getTargetPosition(worldCoords, icbmData.targetWeights, cities, missileBatteries)
        );
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
}

[Serializable]
public struct TargetWeights {
    public float city, battery, random;

    private float sum { get { return city + battery + random; } }

    public TargetType getTargetType(float value) {
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
