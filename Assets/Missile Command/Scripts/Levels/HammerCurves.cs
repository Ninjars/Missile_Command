using UnityEngine;

[CreateAssetMenu(fileName = "Hammer Curves", menuName = "Missile Command/Hammer Curves")]
public class HammerCurves : WeaponCurves { 
    public Hammer prefab;

    [Tooltip("Time before weapon can dodge again.")]
    public AnimationCurve dodgeRecharge;

    [Tooltip("Minimum delay between attack events.")]
    public AnimationCurve rechargeTime;

    [Tooltip("Time taken to launch attack once in position.")]
    public AnimationCurve attackTime;

    [Tooltip("Horizontal speed.")]
    public AnimationCurve speed;

    public Snapshot createSnapshot(float gameProgress) {
        return new Snapshot(
            prefab,
            targetWeights,
            initialDelay,
            intervalVariance,
            Mathf.RoundToInt(count.Evaluate(gameProgress)),
            dodgeRecharge.Evaluate(gameProgress),
            rechargeTime.Evaluate(gameProgress),
            attackTime.Evaluate(gameProgress),
            speed.Evaluate(gameProgress)
        );
    }

    public struct Snapshot : WeaponSnapshot {
        public Snapshot(
            Hammer prefab,
            TargetWeights targetWeights,
            float initialDelay,
            float intervalVariance,
            int count,
            float dodgeRecharge,
            float rechargeTime,
            float attackTime,
            float speed
        ) {
            this.prefab = prefab;
            this.targetWeights = targetWeights;
            this.initialDelay = initialDelay;
            this.intervalVariance = intervalVariance;
            this.count = count;
            this.dodgeRecharge = dodgeRecharge;
            this.rechargeTime = rechargeTime;
            this.attackTime = attackTime;
            this.speed = speed;
        }

        public Hammer prefab { get; }
        public TargetWeights targetWeights { get; }
        public float initialDelay { get; }
        public float intervalVariance { get; }
        public int count { get; }
        public float dodgeRecharge { get; }
        public float rechargeTime { get; }
        public float attackTime { get; }
        public float speed { get; }
    }
}
