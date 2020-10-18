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
            float attackInterval = getAttackInterval(icbmData, stageProgress);
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

    private float getAttackInterval(ICBMData icbmData, float stageProgress) {
        float baseInterval = icbmData.avgInterval.evaluate(stageProgress);
        float maxStageIntervalVariance = icbmData.intervalVariance.evaluate(stageProgress);
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
            getTargetPosition(worldCoords, cities, missileBatteries)
        );
    }

    private Vector2 getTargetPosition(
        WorldCoords worldCoords,
        List<City> cities,
        List<MissileBattery> missileBatteries
    ) {
        City targetCity = cities
                .Where(it => !it.isDestroyed)
                .OrderBy(n => UnityEngine.Random.value)
                .FirstOrDefault();

        if (targetCity != null) {
            return targetCity.transform.position;
        }

        MissileBattery targetBattery = missileBatteries
                .Where(it => !it.getIsDestroyed())
                .OrderBy(n => UnityEngine.Random.value)
                .FirstOrDefault();

        if (targetBattery != null) {
            return targetBattery.transform.position;
        }

        return new Vector2(
            UnityEngine.Random.value * (worldCoords.worldRight - worldCoords.worldLeft) + worldCoords.worldLeft,
            worldCoords.groundY
        );
    }
}
