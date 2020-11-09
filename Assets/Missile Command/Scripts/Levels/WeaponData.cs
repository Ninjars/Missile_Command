using UnityEngine;

public class WeaponData : ScriptableObject {
    public float initialDelay;
    
    public TargetWeights targetWeights;
    
    [Tooltip("Number of instances spawned per level")]
    public RangeIntData count;

    [Tooltip("Variance between spawn intervals. Smaller means more regular spawning. Normalised to even spacing over level time.")]
    public RangeData intervalVariance;
}
