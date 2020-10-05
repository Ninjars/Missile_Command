using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Missile Command/Weapon Data")]
public class WeaponData : ScriptableObject {
    public GameObject weaponPrefab;
    
    [Tooltip("Number of instances spawned per level")]
    public int count;
    
    [Tooltip("Accuracy of the attack for a given target")]
    public LevelFactorData accuracy;
    
    [Tooltip("Average interval between instance spawns. Larger interval leads to a less dense, longer level.")]
    public LevelFactorData avgInterval;

    [Tooltip("Variance between spawn intervals. Smaller means more regular spawning.")]
    public LevelFactorData intervalVariance;

    [Tooltip("Launch impulse of the initial weapon")]
    public LevelFactorData primaryImpulse;

    [Tooltip("Acceleration force of the initial weapon")]
    public LevelFactorData primaryAcceleration;

    [Tooltip("Launch impulse of any sub-munitions")]
    public LevelFactorData secondaryImpulse = null;

    [Tooltip("Acceleration force of any sub-munitions")]
    public LevelFactorData secondaryAcceleration = null;
}
