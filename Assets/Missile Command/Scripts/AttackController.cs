using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackController : MonoBehaviour {

    internal void scheduleAttackEvents(
        WorldCoords worldCoords,
        List<City> cities,
        List<MissileBattery> missileBatteries,
        ICBMData icbmData,
        float stageProgress
    ) {
        float accumulatedTime = 0;
        for (int i = 0; i < icbmData.count.evaluate(stageProgress); i++) {
            float attackInterval = getAttackInterval(icbmData, stageProgress);
            accumulatedTime += attackInterval;
            StartCoroutine(
                scheduleAttack(
                    worldCoords,
                    cities,
                    missileBatteries,
                    accumulatedTime,
                    icbmData,
                    stageProgress
                )
            );
        }
    }

    private float getAttackInterval(ICBMData icbmData, float stageProgress) {
        float baseInterval = icbmData.avgInterval.evaluate(stageProgress);
        float maxStageIntervalVariance = icbmData.intervalVariance.evaluate(stageProgress);
        float variance = maxStageIntervalVariance * UnityEngine.Random.value * 2 - maxStageIntervalVariance;
        return baseInterval + variance;
    }

    private IEnumerator scheduleAttack(
        WorldCoords worldCoords,
        List<City> cities,
        List<MissileBattery> missileBatteries,
        float delay,
        ICBMData icbmData,
        float stageProgress
    ) {
        yield return new WaitForSeconds(delay);

        ICBM weapon = ObjectPoolManager.Instance.getObjectInstance(icbmData.weaponPrefab.gameObject).GetComponent<ICBM>();
        
        Debug.Log("targeting ICBM");
        weapon.launch(
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
            Debug.Log($">> {targetCity}");
            return targetCity.transform.position;
        }

        MissileBattery targetBattery = missileBatteries
                .Where(it => !it.getIsDestroyed())
                .OrderBy(n => UnityEngine.Random.value)
                .FirstOrDefault();

        if (targetBattery != null) {
            Debug.Log($">> {targetBattery}");
            return targetBattery.transform.position;
        }

        Debug.Log(">> ground position");
        return new Vector2(
            UnityEngine.Random.value * (worldCoords.worldRight - worldCoords.worldLeft) + worldCoords.worldLeft,
            worldCoords.groundY
        );
    }
}
