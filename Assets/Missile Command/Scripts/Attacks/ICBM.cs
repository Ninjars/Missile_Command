using System;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class ICBM : Explodable {
    public float worldSpawnBuffer = 1;
    public float layerZ = 5;
    public Explosion explosionPrefab;
    public TrailSettings trailSettings;

    private float speed;
    private Vector3 targetPosition;
    private Vector3 thrustVector;
    private Rigidbody2D _rb;
    private Colors colors { get { return Colors.Instance; } }

    private WorldCoords worldCoords;
    private ICBMCurves.Snapshot data;
    private Func<Vector2> targetProvider;
    private float accuracy;
    private LinearTrail trail;
    private float mirvAltitude;
    private int mirvCountMin;
    private int mirvCountMax;

    private Rigidbody2D rb {
        get {
            if (_rb == null) {
                _rb = GetComponent<Rigidbody2D>();
            }
            return _rb;
        }
    }

    public void launch(
        WorldCoords worldCoords,
        ICBMCurves.Snapshot data,
        Func<Vector2> targetProvider
    ) {
        configure(
            worldCoords,
            data,
            targetProvider,
            accuracy: data.maxDeviation,
            speed: data.speed,
            mirvCountMin: data.mirvCountMin,
            mirvCountMax: data.mirvCountMax,
            mirvAltitude: calculateMirvAltitude(worldCoords, data.mirvChance)
        );
        Vector2 target = targetProvider();
        launch(calculateSpawnPosition(worldCoords, target.x), target);
    }

    public void launch(
        WorldCoords worldCoords,
        ICBMCurves.Snapshot data,
        Vector2 launchPosition,
        Func<Vector2> targetProvider
    ) {
        configure(
            worldCoords,
            data,
            targetProvider,
            accuracy: data.maxDeviation,
            speed: data.speed,
            mirvCountMin: data.mirvCountMin,
            mirvCountMax: data.mirvCountMax,
            mirvAltitude: calculateMirvAltitude(worldCoords, data.mirvChance)
        );
        Vector2 target = targetProvider();
        launch(launchPosition, target);
    }

    private void configure(
        WorldCoords worldCoords,
        ICBMCurves.Snapshot data,
        Func<Vector2> targetProvider,
        float accuracy,
        float speed,
        int mirvCountMin,
        int mirvCountMax,
        float mirvAltitude
    ) {
        this.worldCoords = worldCoords;
        this.data = data;
        this.targetProvider = targetProvider;
        this.accuracy = accuracy;
        this.speed = speed;
        this.mirvCountMin = mirvCountMin;
        this.mirvCountMax = mirvCountMax;
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
        rb.velocity = thrustVector * speed;
        
        transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, rb.velocity, Vector3.back), Vector3.back);

        trail = ObjectPoolManager.Instance.getObjectInstance(trailSettings.prefab.gameObject).GetComponent<LinearTrail>();
        trail.initialise(gameObject, trailSettings, colors.attackTrailColor);
    }

    private float calculateMirvAltitude(WorldCoords worldCoords, float chance) {
        if (UnityEngine.Random.value > chance) {
            return -1;
        } else {
            float dy = worldCoords.worldTop - worldCoords.groundY;
            float min = worldCoords.groundY + dy * 0.4f;
            float max = worldCoords.worldTop - dy * 0.05f;
            return min + UnityEngine.Random.value * (max - min);
        }
    }

    private Vector2 calculateSpawnPosition(WorldCoords worldCoords, float targetX) {
        float x = (worldCoords.width + 2 * worldSpawnBuffer) * UnityEngine.Random.value - worldCoords.worldRight - worldSpawnBuffer;
        return new Vector2(x, worldCoords.worldTop + worldSpawnBuffer);
    }

    public override void explode() {
        gameObject.SetActive(false);

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(transform.position, colors.attackExplodeColor);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        explode();
    }

    private void Update() {
        if (rb.position.y < mirvAltitude) {
            trail.boostDecayTime();
            List<Vector2> selectedTargets = new List<Vector2>(mirvCountMin);
            int mirvCount = Mathf.RoundToInt((mirvCountMax - mirvCountMin) * UnityEngine.Random.value + mirvCountMin);
            for (int i = 0; i < mirvCount; i++) {
                ICBM weapon = ObjectPoolManager.Instance.getObjectInstance(data.prefab.gameObject).GetComponent<ICBM>();
                weapon.configure(
                    worldCoords,
                    data,
                    targetProvider,
                    accuracy: accuracy,
                    speed: speed,
                    mirvCountMin: 0,
                    mirvCountMax: 0,
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
