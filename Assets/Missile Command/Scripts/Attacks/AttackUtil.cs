using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUtil {
    public IEnumerator scheduleIcbmAttack(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        float delay,
        ICBMData icbmData,
        float stageProgress,
        Func<Vector2> targetProvider
    ) {
        yield return new WaitForSeconds(delay);

        ICBM weapon = ObjectPoolManager.Instance.getObjectInstance(icbmData.weaponPrefab.gameObject).GetComponent<ICBM>();
        
        weapon.launch(
            stateUpdater,
            worldCoords,
            icbmData,
            stageProgress,
            targetProvider
        );
    }

    public IEnumerator scheduleBomberAttack(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        float delay,
        BomberData weaponData,
        float stageProgress,
        Func<Vector2> targetProvider
    ) {
        int bomberCount = weaponData.bombersPerWing.evaluate(stageProgress);
        float altitude = calculateBomberAltitude(worldCoords);
        float x = UnityEngine.Random.value <= 0.5f
                ? worldCoords.worldLeft - 1
                : worldCoords.worldRight + 1;
        return spawnBombers(
            stateUpdater,
            worldCoords,
            delay,
            weaponData,
            stageProgress,
            targetProvider,
            bomberCount,
            x,
            altitude
        );
    }

    private IEnumerator spawnBombers(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        float delay,
        BomberData weaponData,
        float stageProgress,
        Func<Vector2> targetProvider,
        int bomberCount,
        float x,
        float altitude
    ) {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < bomberCount; i++) {
            spawnBomber(
                stateUpdater,
                worldCoords,
                weaponData,
                stageProgress,
                targetProvider,
                x, 
                altitude + (UnityEngine.Random.value * 2 - 1) * 0.5f
            );
            yield return new WaitForSeconds(1);
        }
    }

    private void spawnBomber(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        BomberData weaponData,
        float stageProgress,
        Func<Vector2> targetProvider,
        float x, 
        float y
    ) {
        Bomber weapon = ObjectPoolManager.Instance.getObjectInstance(weaponData.weaponPrefab.gameObject).GetComponent<Bomber>();
        weapon.launch(
            stateUpdater,
            worldCoords,
            weaponData,
            stageProgress,
            targetProvider,
            x,
            y
        );
    }

    private float calculateBomberAltitude(WorldCoords worldCoords) {
        float dy = worldCoords.worldTop - worldCoords.groundY;
        float min = worldCoords.groundY + dy * 0.33f;
        float max = worldCoords.worldTop - dy * 0.45f;
        return min + UnityEngine.Random.value * (max - min);
    }
}
