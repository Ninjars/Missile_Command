﻿using UnityEngine;

[CreateAssetMenu(fileName = "ICBM Data", menuName = "Missile Command/ICBM")]
public class ICBMData : ScriptableObject {
    public ICBM weaponPrefab;
    
    [Tooltip("Number of instances spawned per level")]
    public RangeIntData count;
    
    [Tooltip("Accuracy of the attack for a given target")]
    public RangeData accuracy;
    
    [Tooltip("Average interval between instance spawns. Larger interval leads to a less dense, longer level.")]
    public RangeData avgInterval;

    [Tooltip("Variance between spawn intervals. Smaller means more regular spawning.")]
    public RangeData intervalVariance;

    [Tooltip("Launch impulse of the initial weapon")]
    public RangeData primaryImpulse;

    [Tooltip("Acceleration force of the initial weapon")]
    public RangeData primaryAcceleration;
    [Tooltip("Chance of primary warhead splitting into multiple secondaries at some point during its descent")]
    public RangeData mirvChance;
    [Tooltip("Number of secondary warheads produced if warhead goes MIRV")]
    public RangeIntData mirvCount;
}
