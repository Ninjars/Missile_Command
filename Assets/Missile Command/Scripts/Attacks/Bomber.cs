using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class Bomber : MonoBehaviour {
    public float worldSpawnBuffer = 1;
    public float layerZ = 6;

    public Explosion explosionPrefab;

    private float chargeTime;
    private float evasionSpeed;
    private float speed;
    private Vector2 velocity;
    private Colors colors { get { return Colors.Instance; } }

    private WorldCoords worldCoords;
    private StateUpdater stateUpdater;
    private Func<Vector3, Vector2> targetProvider;
    private float stageProgress;
    private ICBMData bombAttackData;
    private Coroutine attackRoutine;
    private Transform visualsTransform;

    private Rigidbody2D _rb;

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
        BomberData weaponData,
        float stageProgress,
        Func<Vector3, Vector2> targetProvider,
        float x,
        float y
    ) {
        this.stateUpdater = stateUpdater;
        this.targetProvider = targetProvider;
        this.stageProgress = stageProgress;
        this.worldCoords = worldCoords;
        this.chargeTime = weaponData.chargeTime.evaluate(stageProgress);
        this.evasionSpeed = weaponData.evasionSpeed.evaluate(stageProgress);
        this.speed = weaponData.speed.evaluate(stageProgress);
        this.bombAttackData = weaponData.bombAttackData;
        
        Polyline visuals = GetComponentInChildren<Polyline>();
        visualsTransform = visuals.transform;
        visuals.Color = colors.attackColor;
        
        velocity = x < worldCoords.centerX 
                ? new Vector2(speed, 0)
                : new Vector2(-speed, 0);

                
        transform.rotation = Quaternion.Euler(0, 0, x < worldCoords.centerX ? -90 : 90);

        transform.position = new Vector3(x, y, layerZ);
        gameObject.SetActive(true);

        rb.velocity = velocity;
        rb.angularVelocity = 0;

        attackRoutine = StartCoroutine(attackLoop());
    }

    private IEnumerator attackLoop() {
        yield return new WaitForSeconds(chargeTime);
        while (gameObject.activeInHierarchy) {
            if (transform.position.x > worldCoords.worldLeft && transform.position.x < worldCoords.worldRight) {
                float xVel = rb.velocity.x > 0 ? 1 : -1;
                launchAttack(new Vector3(transform.position.x, transform.position.y, xVel), targetProvider(new Vector3(transform.position.x, transform.position.y, xVel)));
                yield return new WaitForSeconds(chargeTime);

            } else {
                // wait until back on screen
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void launchAttack(Vector3 launchPosition, Vector2 targetPosition) {
        ICBM weapon = ObjectPoolManager.Instance.getObjectInstance(bombAttackData.weaponPrefab.gameObject).GetComponent<ICBM>();
        weapon.launch(
            stateUpdater,
            worldCoords,
            bombAttackData,
            stageProgress,
            launchPosition,
            () => targetPosition
        );
    }

    private void Update() {
        if (transform.position.x < worldCoords.worldLeft - worldSpawnBuffer) {
            rb.velocity = new Vector2(speed, 0);
            transform.rotation = Quaternion.Euler(0, 0, -90);

        } else if (transform.position.x > worldCoords.worldRight + worldSpawnBuffer) {
            rb.velocity = new Vector2(-speed, 0);
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log($"OnCollisionEnter2D {gameObject.name} -> {other.gameObject.name} on layer {LayerMask.LayerToName(other.gameObject.layer)}");
        explode();
    }

    private void explode() {
        gameObject.SetActive(false);

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(transform.position, colors.attackExplodeColor);
    }

    private void OnDisable() {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        if (attackRoutine != null) {
            StopCoroutine(attackRoutine);
        }
    }
}
