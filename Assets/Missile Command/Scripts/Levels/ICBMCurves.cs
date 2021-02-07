using UnityEngine;

[CreateAssetMenu(fileName = "ICBM Curves", menuName = "Missile Command/ICBM Curves")]
public class ICBMCurves : WeaponCurves {
    public ICBM prefab;
    
    [Tooltip("Horizontal distance the attack can vary from intended target.")]
    public AnimationCurve maxDeviation;
    
    [Tooltip("Speed of the weapon.")]
    public AnimationCurve speed;
    
    [Tooltip("Chance of primary warhead splitting into multiple secondaries at some point during its descent.")]
    public AnimationCurve mirvChance;
    
    [Tooltip("Number of secondary warheads produced if warhead goes MIRV.")]
    public AnimationCurve mirvCountMin;
    public AnimationCurve mirvCountMax;

    public Snapshot createSnapshot(float gameProgress) {
        return new Snapshot(
            prefab,
            targetWeights,
            initialDelay,
            intervalVariance,
            Mathf.RoundToInt(count.Evaluate(gameProgress)),
            maxDeviation.Evaluate(gameProgress),
            speed.Evaluate(gameProgress),
            mirvChance.Evaluate(gameProgress),
            Mathf.RoundToInt(mirvCountMin.Evaluate(gameProgress)),
            Mathf.RoundToInt(mirvCountMax.Evaluate(gameProgress))
        );
    }

    public struct Snapshot : WeaponSnapshot {
        public Snapshot(
            ICBM prefab,
            TargetWeights targetWeights,
            float initialDelay,
            float intervalVariance,
            int count,
            float maxDeviation,
            float speed,
            float mirvChance,
            int mirvCountMin,
            int mirvCountMax
        ) {
            this.prefab = prefab;
            this.targetWeights = targetWeights;
            this.initialDelay = initialDelay;
            this.intervalVariance = intervalVariance;
            this.count = count;
            this.maxDeviation = maxDeviation;
            this.speed = speed;
            this.mirvChance = mirvChance;
            this.mirvCountMin = mirvCountMin;
            this.mirvCountMax = mirvCountMax;
        }

        public ICBM prefab { get; }
        public TargetWeights targetWeights { get; }
        public float initialDelay { get; }
        public float intervalVariance { get; }
        public int count { get; }
        public float maxDeviation { get; }
        public float speed { get; }
        public float mirvChance { get; }
        public int mirvCountMin { get; }
        public int mirvCountMax { get; }
    }
}
