using UnityEngine;

[CreateAssetMenu(fileName = "Bomber Curves", menuName = "Missile Command/Bomber Curves")]
public class BomberCurves : WeaponCurves {
    public Bomber prefab;

    [Tooltip("Weapon launched by the bomber")]
    public ICBMCurves bombAttackData;

    [Tooltip("Number of bombers per spawn, following same flight path.")]
    public AnimationCurve bombersPerWing;

    [Tooltip("Delay between bomb events.")]
    public AnimationCurve chargeTime;

    [Tooltip("Horizontal speed.")]
    public AnimationCurve speed;

    [Tooltip("Altitude range, as factor of full world height. 1 = world top, 0 = world bottom. Bomber will spawn randomly in this range.")]
    public float altitudeMin;
    [Tooltip("Altitude range, as factor of full world height. 1 = world top, 0 = world bottom. Bomber will spawn randomly in this range.")]
    public float altitudeMax;

    [Tooltip("Ability of bomber to avoid explosions.")]
    public AnimationCurve evasionSpeed;

    public Snapshot createSnapshot(float gameProgress) {
        return new Snapshot(
            prefab,
            bombAttackData.createSnapshot(gameProgress),
            initialDelay,
            intervalVariance,
            Mathf.RoundToInt(count.Evaluate(gameProgress)),
            Mathf.RoundToInt(bombersPerWing.Evaluate(gameProgress)),
            chargeTime.Evaluate(gameProgress),
            speed.Evaluate(gameProgress),
            altitudeMin,
            altitudeMax,
            evasionSpeed.Evaluate(gameProgress)
        );
    }

    public struct Snapshot : WeaponSnapshot {
        public Snapshot(
            Bomber prefab,
            ICBMCurves.Snapshot bombSnapshot,
            float initialDelay,
            float intervalVariance,
            int count,
            int bombersPerWing,
            float chargeTime,
            float speed,
            float altitudeMin,
            float altitudeMax,
            float evasionSpeed
        ) {
            this.prefab = prefab;
            this.bombSnapshot = bombSnapshot;
            this.intervalVariance = intervalVariance;
            this.initialDelay = initialDelay;
            this.count = count;
            this.bombersPerWing = bombersPerWing;
            this.chargeTime = chargeTime;
            this.speed = speed;
            this.altitudeMin = altitudeMin;
            this.altitudeMax = altitudeMax;
            this.evasionSpeed = evasionSpeed;
        }

        public Bomber prefab { get; }
        public ICBMCurves.Snapshot bombSnapshot { get; }
        public float initialDelay { get; }
        public float intervalVariance { get; }
        public int count { get; }
        public int bombersPerWing { get; }
        public float chargeTime { get; }
        public float speed { get; }
        public float altitudeMin { get; }
        public float altitudeMax { get; }
        public float evasionSpeed { get; }
        public TargetWeights targetWeights { get { return new TargetWeights(); }}
    }
}

