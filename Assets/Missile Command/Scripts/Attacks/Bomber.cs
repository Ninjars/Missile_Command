using System;
using UnityEngine;

public class Bomber : MonoBehaviour {
    public float worldSpawnBuffer = 1;
    public float layerZ = 6;

    public Explosion explosionPrefab;

    private float chargeTime;
    private float evasionSpeed;
    private float speed;
    private float shotDeviation;
    private Vector2 velocity;
    private Colors colors { get { return Colors.Instance; } }

    private WorldCoords worldCoords;
    private StateUpdater stateUpdater;
    private BomberData weaponData;
    private Func<Vector2> targetProvider;

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
        Func<Vector2> targetProvider,
        float x,
        float y
    ) {
        this.stateUpdater = stateUpdater;
        this.weaponData = weaponData;
        this.targetProvider = targetProvider;
        this.chargeTime = weaponData.chargeTime.evaluate(stageProgress);
        this.evasionSpeed = weaponData.evasionSpeed.evaluate(stageProgress);
        this.speed = weaponData.speed.evaluate(stageProgress);
        
        velocity = x < worldCoords.centerX 
                ? new Vector2(speed, 0)
                : new Vector2(-speed, 0);

        transform.position = new Vector3(x, y, layerZ);
        gameObject.SetActive(true);

        rb.velocity = velocity;
        rb.angularVelocity = 0;
    }

    private void Update() {
        if (transform.position.x < worldCoords.worldLeft - 1) {
            rb.velocity = new Vector2(speed, 0);

        } else if (transform.position.x > worldCoords.worldRight + 1) {
            rb.velocity = new Vector2(-speed, 0);
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
    }
}
