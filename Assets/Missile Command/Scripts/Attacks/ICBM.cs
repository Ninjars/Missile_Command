using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class ICBM : MonoBehaviour {
    public float worldSpawnBuffer = 1;
    public float layerZ = 5;
    public Explosion explosionPrefab;
    public TrailSettings trailSettings;

    private float thrust;
    private float impulse;
    private Vector3 targetPosition;
    private Vector3 thrustVector;
    private Rigidbody2D _rb;
    private Colors colors { get { return Colors.Instance; } }

    private WorldCoords worldCoords;
    private StateUpdater stateUpdater;
    private ICBMData weaponData;
    private Func<Vector2> targetProvider;
    private float accuracy;
    private LinearTrail trail;
    private float mirvAltitude;
    private int mirvCount;

    private Rigidbody2D rb {
        get {
            if (_rb == null) {
                _rb = GetComponent<Rigidbody2D>();
            }
            return _rb;
        }
    }

    public void launch(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        ICBMData weaponData,
        float stageProgress,
        Func<Vector2> targetProvider
    ) {
        configure(
            worldCoords,
            stateUpdater,
            weaponData,
            targetProvider,
            accuracy: weaponData.maxDeviation.evaluate(stageProgress),
            thrust: weaponData.primaryAcceleration.evaluate(stageProgress),
            impulse: weaponData.primaryImpulse.evaluate(stageProgress),
            mirvCount: weaponData.mirvCount.evaluate(stageProgress),
            mirvAltitude: calculateMirvAltitude(worldCoords, weaponData.mirvChance.evaluate(stageProgress))
        );
        Vector2 target = targetProvider();
        launch(calculateSpawnPosition(worldCoords, target.x), target);
    }

    public void launch(
        StateUpdater stateUpdater,
        WorldCoords worldCoords,
        ICBMData weaponData,
        float stageProgress,
        Vector2 launchPosition,
        Func<Vector2> targetProvider
    ) {
        configure(
            worldCoords,
            stateUpdater,
            weaponData,
            targetProvider,
            accuracy: weaponData.maxDeviation.evaluate(stageProgress),
            thrust: weaponData.primaryAcceleration.evaluate(stageProgress),
            impulse: weaponData.primaryImpulse.evaluate(stageProgress),
            mirvCount: weaponData.mirvCount.evaluate(stageProgress),
            mirvAltitude: calculateMirvAltitude(worldCoords, weaponData.mirvChance.evaluate(stageProgress))
        );
        Vector2 target = targetProvider();
        launch(launchPosition, target);
    }

    private void configure(
        WorldCoords worldCoords,
        StateUpdater stateUpdater,
        ICBMData weaponData,
        Func<Vector2> targetProvider,
        float accuracy,
        float thrust,
        float impulse,
        int mirvCount,
        float mirvAltitude
    ) {
        this.worldCoords = worldCoords;
        this.stateUpdater = stateUpdater;
        this.weaponData = weaponData;
        this.targetProvider = targetProvider;
        this.accuracy = accuracy;
        this.thrust = thrust;
        this.impulse = impulse;
        this.mirvCount = mirvCount;
        this.mirvAltitude = mirvAltitude;
    }

    private void launch(Vector2 spawnPosition, Vector2 target) {
        float deviance = UnityEngine.Random.value * accuracy;
        if (UnityEngine.Random.value < 0.5f) {
            deviance = -deviance;
        }
        this.targetPosition = new Vector3(target.x + deviance, target.y, layerZ);

        transform.position = new Vector3(spawnPosition.x, spawnPosition.y, layerZ);
        GetComponentInChildren<Polyline>().Color = colors.attackColor;
        gameObject.SetActive(true);

        this.thrustVector = (targetPosition - transform.position).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(thrustVector * impulse);
        
        transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, rb.velocity, Vector3.back), Vector3.back);

        trail = ObjectPoolManager.Instance.getObjectInstance(trailSettings.prefab.gameObject).GetComponent<LinearTrail>();
        trail.initialise(gameObject, trailSettings, colors.attackTrailColor);
    }

    private float calculateMirvAltitude(WorldCoords worldCoords, float chance) {
        if (UnityEngine.Random.value > chance) {
            return -1;
        } else {
            float dy = worldCoords.worldTop - worldCoords.groundY;
            float min = worldCoords.groundY + dy * 0.33f;
            float max = worldCoords.worldTop - dy * 0.05f;
            return min + UnityEngine.Random.value * (max - min);
        }
    }

    private Vector2 calculateSpawnPosition(WorldCoords worldCoords, float targetX) {
        // float x;
        // if (targetX > worldCoords.centerX) {
        //     x = worldCoords.worldLeft - worldSpawnBuffer;
        // } else {
        //     x = worldCoords.worldRight + worldSpawnBuffer;
        // }
        // x *= UnityEngine.Random.value
        float x = (worldCoords.width + 2 * worldSpawnBuffer) * UnityEngine.Random.value - worldCoords.worldRight - worldSpawnBuffer;
        return new Vector2(x, worldCoords.worldTop + worldSpawnBuffer);
    }

    private void explode() {
        gameObject.SetActive(false);

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(transform.position, colors.attackExplodeColor);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log($"OnCollisionEnter2D {gameObject.name} -> {other.gameObject.name} on layer {LayerMask.LayerToName(other.gameObject.layer)}");
        explode();
    }


    private void FixedUpdate() {
        rb.AddForce(thrustVector * thrust, ForceMode2D.Force);
    }

    private void Update() {
        if (rb.position.y < mirvAltitude) {
            trail.boostDecayTime();
            List<Vector2> selectedTargets = new List<Vector2>(mirvCount);
            for (int i = 0; i < mirvCount; i++) {
                ICBM weapon = ObjectPoolManager.Instance.getObjectInstance(weaponData.weaponPrefab.gameObject).GetComponent<ICBM>();
                weapon.configure(
                    worldCoords,
                    stateUpdater,
                    weaponData,
                    targetProvider,
                    accuracy: accuracy,
                    thrust: thrust,
                    impulse: impulse,
                    mirvCount: 0,
                    mirvAltitude: -1
                );
                weapon.launch(transform.position, targetProvider.Invoke());
            }
            gameObject.SetActive(false);
        }
    }

    private void OnDisable() {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        if (trail != null) {
            trail.onSubjectDisabled();
            trail = null;
        }
    }

    // private void OnDrawGizmos() {
    //     Debug.DrawLine(rb.position, (Vector2) targetPosition, Color.red);
    //     Debug.DrawLine(rb.position, (Vector3) rb.position + thrustVector * 2, Color.green);
    // }
}
