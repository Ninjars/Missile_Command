using System;
using System.Collections;
using UnityEngine;

public class AttackUtil {
    public static IEnumerator scheduleIcbmAttack(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        float delay,
        ICBMData icbmData,
        float stageProgress,
        Action onLaunchCallback,
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
        onLaunchCallback();
    }

    public static IEnumerator scheduleBomberAttack(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        float delay,
        BomberData weaponData,
        float stageProgress,
        Action onLaunchCallback,
        Func<Vector3, Vector2> targetProvider
    ) {
        int bomberCount = weaponData.bombersPerWing.evaluate(stageProgress);
        float altitude = calculateBomberAltitude(worldCoords, weaponData.altitude);
        float x = UnityEngine.Random.value <= 0.5f
                ? worldCoords.worldLeft - weaponData.weaponPrefab.worldSpawnBuffer
                : worldCoords.worldRight + weaponData.weaponPrefab.worldSpawnBuffer;
        return spawnBombers(
            stateUpdater,
            worldCoords,
            delay,
            weaponData,
            stageProgress,
            onLaunchCallback,
            targetProvider,
            bomberCount,
            x,
            altitude
        );
    }

    private static IEnumerator spawnBombers(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        float delay,
        BomberData weaponData,
        float stageProgress,
        Action onLaunchCallback,
        Func<Vector3, Vector2> targetProvider,
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
        onLaunchCallback();
    }

    private static void spawnBomber(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        BomberData weaponData,
        float stageProgress,
        Func<Vector3, Vector2> targetProvider,
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

    private static float calculateBomberAltitude(WorldCoords worldCoords, RangeData altitude) {
        float dy = worldCoords.worldTop - worldCoords.groundY;
        float altOffset = altitude.evaluate(UnityEngine.Random.value);
        return worldCoords.groundY + altOffset * dy;
    }
}
