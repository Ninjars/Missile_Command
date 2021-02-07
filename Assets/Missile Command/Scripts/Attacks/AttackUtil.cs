using System;
using System.Collections;
using UnityEngine;

public class AttackUtil {
    public static IEnumerator scheduleHammerAttack(
        WorldCoords worldCoords,
        float delay,
        HammerCurves.Snapshot data,
        Action onLaunchCallback,
        Func<Vector2> targetProvider
    ) {
        yield return new WaitForSeconds(delay);

        Hammer weapon = ObjectPoolManager.Instance.getObjectInstance(data.prefab.gameObject).GetComponent<Hammer>();

        weapon.launch(
            worldCoords,
            data,
            targetProvider
        );
        onLaunchCallback();
    }

    public static IEnumerator scheduleIcbmAttack(
        WorldCoords worldCoords,
        float delay,
        ICBMCurves.Snapshot data,
        Action onLaunchCallback,
        Func<Vector2> targetProvider
    ) {
        yield return new WaitForSeconds(delay);

        ICBM weapon = ObjectPoolManager.Instance.getObjectInstance(data.prefab.gameObject).GetComponent<ICBM>();
        
        weapon.launch(
            worldCoords,
            data,
            targetProvider
        );
        onLaunchCallback();
    }

    public static IEnumerator scheduleBomberAttack(
        WorldCoords worldCoords,
        float delay,
        BomberCurves.Snapshot data,
        Action onLaunchCallback,
        Func<Vector3, Vector2> targetProvider
    ) {
        int bomberCount = data.bombersPerWing;
        float altitude = calculateBomberAltitude(worldCoords, data.altitudeMin, data.altitudeMax);
        float x = UnityEngine.Random.value <= 0.5f
                ? worldCoords.worldLeft - 1
                : worldCoords.worldRight + 1;
        return spawnBombers(
            worldCoords,
            delay,
            data,
            onLaunchCallback,
            targetProvider,
            bomberCount,
            x,
            altitude
        );
    }

    private static IEnumerator spawnBombers(
        WorldCoords worldCoords,
        float delay,
        BomberCurves.Snapshot data,
        Action onLaunchCallback,
        Func<Vector3, Vector2> targetProvider,
        int bomberCount,
        float x,
        float altitude
    ) {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < bomberCount; i++) {
            spawnBomber(
                worldCoords,
                data,
                targetProvider,
                getMaxBomberAltitude(worldCoords, data.altitudeMax),
                x, 
                altitude + (UnityEngine.Random.value * 2 - 1) * 0.5f
            );
            yield return new WaitForSeconds(1);
        }
        onLaunchCallback();
    }

    private static void spawnBomber(
        WorldCoords worldCoords,
        BomberCurves.Snapshot data,
        Func<Vector3, Vector2> targetProvider,
        float maxAltitude,
        float x, 
        float y
    ) {
        Bomber weapon = ObjectPoolManager.Instance.getObjectInstance(data.prefab.gameObject).GetComponent<Bomber>();
        weapon.launch(
            worldCoords,
            data,
            targetProvider,
            maxAltitude,
            x,
            y
        );
    }

    private static float calculateBomberAltitude(WorldCoords worldCoords, float min, float max) {
        float dy = worldCoords.worldTop - worldCoords.groundY;
        float altOffset = min + ((max - min) * UnityEngine.Random.value);
        return worldCoords.groundY + altOffset * dy;
    }

    private static float getMaxBomberAltitude(WorldCoords worldCoords, float max) {
        float dy = worldCoords.worldTop - worldCoords.groundY;
        return worldCoords.groundY + max * dy;
    }
}
