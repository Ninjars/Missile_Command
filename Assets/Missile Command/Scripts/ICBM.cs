﻿using System;
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
    private float deviance;
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
        Vector2 targetCoords
    ) {
        float deviance = UnityEngine.Random.value * weaponData.accuracy.evaluate(stageProgress);
        if (UnityEngine.Random.value < 0.5f) {
            deviance = -deviance;
        }
        configure(
            worldCoords,
            stateUpdater,
            weaponData,
            thrust: weaponData.primaryAcceleration.evaluate(stageProgress),
            impulse: weaponData.primaryImpulse.evaluate(stageProgress),
            deviance,
            mirvCount: weaponData.mirvCount.evaluate(stageProgress),
            mirvAltitude: calculateMirvAltitude(worldCoords, weaponData.mirvChance.evaluate(stageProgress))
        );

        launch(
            calculateSpawnPosition(worldCoords, targetCoords.x),
            new Vector3(targetCoords.x + deviance, targetCoords.y, layerZ)
        );
    }

    private void configure(
        WorldCoords worldCoords,
        StateUpdater stateUpdater,
        ICBMData weaponData,
        float thrust,
        float impulse,
        float deviance,
        int mirvCount,
        float mirvAltitude
    ) {
        this.worldCoords = worldCoords;
        this.stateUpdater = stateUpdater;
        this.weaponData = weaponData;
        this.deviance = deviance;
        this.thrust = thrust;
        this.impulse = impulse;
        this.mirvAltitude = mirvAltitude;
        this.mirvCount = mirvCount;
    }

    private void launch(Vector3 spawnPosition, Vector3 targetPosition) {
        transform.position = spawnPosition;
        GetComponentInChildren<Polyline>().Color = colors.attackColor;
        gameObject.SetActive(true);

        this.targetPosition = targetPosition;
        this.thrustVector = (targetPosition - spawnPosition).normalized;
        rb.AddForce(thrustVector * impulse);

        trail = ObjectPoolManager.Instance.getObjectInstance(trailSettings.prefab.gameObject).GetComponent<LinearTrail>();
        trail.initialise(gameObject, trailSettings, colors.attackTrailColor);
    }

    private float calculateMirvAltitude(WorldCoords worldCoords, float chance) {
        if (UnityEngine.Random.value > chance) {
            return -1;
        } else {
            float dy = worldCoords.worldTop - worldCoords.groundY;
            float min = worldCoords.groundY + dy * 0.33f;
            float max = worldCoords.worldTop - dy * 0.33f;
            return min + UnityEngine.Random.value * (max - min);
        }
    }

    private Vector3 calculateSpawnPosition(WorldCoords worldCoords, float targetX) {
        float x;
        if (targetX > worldCoords.centerX) {
            x = worldCoords.worldLeft - worldSpawnBuffer;
        } else {
            x = worldCoords.worldRight + worldSpawnBuffer;
        }
        return new Vector3(x * UnityEngine.Random.value, worldCoords.worldTop + worldSpawnBuffer, layerZ);
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
            for (int i = 0; i < mirvCount; i++) {
                ICBM weapon = ObjectPoolManager.Instance.getObjectInstance(weaponData.weaponPrefab.gameObject).GetComponent<ICBM>();
                weapon.configure(
                    worldCoords,
                    stateUpdater,
                    weaponData,
                    thrust,
                    impulse,
                    deviance,
                    0,
                    -1
                );
                Vector3 spawnPosition = transform.position;
                Vector3 targetPosition = new Vector3(worldCoords.worldLeft + worldCoords.width * UnityEngine.Random.value, 0, layerZ);
                weapon.launch(spawnPosition, targetPosition);
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
}
