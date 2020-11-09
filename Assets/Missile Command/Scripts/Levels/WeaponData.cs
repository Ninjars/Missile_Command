using UnityEngine;

public class WeaponData : ScriptableObject {
    public float initialDelay;
    
    public TargetWeights targetWeights;
    
    [Tooltip("Number of instances spawned per level")]
    public RangeIntData count;
    
    [Tooltip("Average interval between instance spawns. Larger interval leads to a less dense, longer level.")]
    public RangeData avgInterval;

    [Tooltip("Variance between spawn intervals. Smaller means more regular spawning.")]
    public RangeData intervalVariance;
}
