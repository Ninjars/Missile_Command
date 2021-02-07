using System;
using System.Collections;
using Shapes;
using UnityEngine;

public class Bomber : Explodable {
    public float worldSpawnBuffer = 1;
    public float layerZ = 6;

    public Explosion explosionPrefab;

    private float chargeTime;
    private float evasionSpeed;
    private float speed;
    private Vector2 velocity;
    private Colors colors { get { return Colors.Instance; } }

    private WorldCoords worldCoords;
    private Func<Vector3, Vector2> targetProvider;
    private ICBMCurves.Snapshot bombAttackData;
    private Coroutine attackRoutine;
    private Polyline _visuals;
    private Polyline visuals {
        get {
            if (_visuals == null) {
                _visuals = GetComponentInChildren<Polyline>();
            }
            return _visuals;
        }
    }

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
        WorldCoords worldCoords,
        BomberCurves.Snapshot data,
        Func<Vector3, Vector2> targetProvider,
        float x,
        float y
    ) {
        this.targetProvider = targetProvider;
        this.worldCoords = worldCoords;
        this.chargeTime = data.chargeTime;
        this.evasionSpeed = data.evasionSpeed;
        this.speed = data.speed;
        this.bombAttackData = data.bombSnapshot;
        
        velocity = x < worldCoords.centerX 
                ? new Vector2(speed, 0)
                : new Vector2(-speed, 0);
                
        transform.rotation = Quaternion.Euler(0, 0, x < worldCoords.centerX ? -90 : 90);

        visuals.Color = colors.attackColor;
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
        ICBM weapon = ObjectPoolManager.Instance.getObjectInstance(bombAttackData.prefab.gameObject).GetComponent<ICBM>();
        weapon.launch(
            worldCoords,
            bombAttackData,
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
        explode();
    }

    public override void explode() {
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
