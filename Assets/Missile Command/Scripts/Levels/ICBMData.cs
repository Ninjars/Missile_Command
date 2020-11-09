using UnityEngine;

[CreateAssetMenu(fileName = "ICBM Data", menuName = "Missile Command/ICBM")]
public class ICBMData : WeaponData {
    public ICBM weaponPrefab;
    
    [Tooltip("Horizontal distance the attack can vary from intended target")]
    public RangeData maxDeviation;
    
    [Tooltip("Launch impulse of the initial weapon")]
    public RangeData primaryImpulse;

    [Tooltip("Acceleration force of the initial weapon")]
    public RangeData primaryAcceleration;
    
    [Tooltip("Chance of primary warhead splitting into multiple secondaries at some point during its descent")]
    public RangeData mirvChance;
    
    [Tooltip("Number of secondary warheads produced if warhead goes MIRV")]
    public RangeIntData mirvCount;
}
