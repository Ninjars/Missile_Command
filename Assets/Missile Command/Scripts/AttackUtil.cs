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
}
