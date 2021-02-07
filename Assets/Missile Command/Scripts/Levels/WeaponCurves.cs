using UnityEngine;

public class WeaponCurves : ScriptableObject {
    public float initialDelay;
    
    public TargetWeights targetWeights;
    
    [Tooltip("Number of instances spawned per level")]
    public AnimationCurve count;

    [Tooltip("Variance between spawn intervals. Smaller means more regular spawning. Normalised to even spacing over level time.")]
    public float intervalVariance;
}
