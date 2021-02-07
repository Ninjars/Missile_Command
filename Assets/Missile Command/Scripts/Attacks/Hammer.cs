using System;
using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;
public class Hammer : Explodable {
    public float worldSpawnBuffer = 1;
    public float layerZ = 4;
    public float worldHeightFactor = 0.95f;
    public float dodgeCheckRadius = 0.25f;
    public float dodgeCheckDistance = 1f;
    public LayerMask dodgeCheckLayerMask;

    public Explosion explosionPrefab;
    public HammerBeam beamAttackPrefab;
    private HammerBeam beamAttack;
    private WorldCoords worldCoords;
    private Func<Vector2> targetProvider;
    private float dodgeRechargeTime;
    private float attackRechargeTime;
    private float attackTime;
    private float speed;
    private bool dodgeIsCharged;
    private bool attackIsCharged;

    private Colors colors { get { return Colors.Instance; } }
    private float targetX;
    private List<Coroutine> activeRoutines;
    private RaycastHit2D[] dodgeCheckResults;
    private Rigidbody2D _rb;
    private Rigidbody2D rb {
        get {
            if (_rb == null) {
                _rb = GetComponent<Rigidbody2D>();
            }
            return _rb;
        }
    }
    private State state;

    private enum State {
        EVADING,
        MOVING_TO_ATTACK,
        ATTACKING
    }

    public void launch(
        WorldCoords worldCoords,
        HammerData weaponData,
        float stageProgress,
        Func<Vector2> targetProvider
    ) {
        this.targetProvider = targetProvider;
        this.worldCoords = worldCoords;
        dodgeRechargeTime = weaponData.dodgeRecharge.evaluate(stageProgress);
        attackRechargeTime = weaponData.rechargeTime.evaluate(stageProgress);
        attackTime = weaponData.attackTime.evaluate(stageProgress);
        speed = weaponData.speed.evaluate(stageProgress);
        state = State.EVADING;
        dodgeIsCharged = false;
        attackIsCharged = false;
        activeRoutines = new List<Coroutine>();
        dodgeCheckResults = new RaycastHit2D[1];

        bool isLeftSpawn = UnityEngine.Random.value < 0.5;
        Vector3 spawnPosition = new Vector3(
            isLeftSpawn ? worldCoords.worldLeft - worldSpawnBuffer : worldCoords.worldRight + worldSpawnBuffer,
            worldCoords.worldTop * worldHeightFactor,
            layerZ
        );
        transform.position = spawnPosition;
        
        foreach (var shape in GetComponentsInChildren<ShapeRenderer>()) {
            shape.Color = colors.attackColor;
        }

        gameObject.SetActive(true);
        rb.velocity = isLeftSpawn
                ? new Vector2(speed, 0)
                : new Vector2(-speed, 0);
        rb.angularVelocity = 0;
        activeRoutines.Add(StartCoroutine(rechargeDodge()));
        activeRoutines.Add(StartCoroutine(rechargeAttack()));
    }

    private IEnumerator rechargeDodge() {
        yield return new WaitForSeconds(dodgeRechargeTime);
        dodgeIsCharged = true;
    }

    private IEnumerator rechargeAttack() {
        yield return new WaitForSeconds(attackRechargeTime);
        attackIsCharged = true;
    }

    private IEnumerator executeAttackSequence() {
        beamAttack = GameObject.Instantiate<HammerBeam>(beamAttackPrefab);
        beamAttack.beginSequence(rb.position, worldCoords.groundY, attackTime);

        yield return new WaitForSeconds(attackTime + 1f);

        state = Hammer.State.EVADING;
        attackIsCharged = false;
        rb.velocity = UnityEngine.Random.value < 0.5f
                ? new Vector2(speed, 0)
                : new Vector2(-speed, 0);
        activeRoutines.Add(StartCoroutine(rechargeDodge()));
        activeRoutines.Add(StartCoroutine(rechargeAttack()));
    }

    private void OnCollisionEnter2D(Collision2D other) {
        explode();
    }

    public override void explode() {
        if (beamAttack != null) {
            beamAttack.onInterrupt();
        }
        gameObject.SetActive(false);

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(transform.position, colors.attackExplodeColor);
    }

    private void Update() {
        switch (state) {
            case Hammer.State.EVADING: {
                    // check if out of bounds
                    if (rb.position.x < worldCoords.worldLeft - worldSpawnBuffer) {
                        Debug.Log("reached left bounds");
                        rb.velocity = new Vector2(speed, 0);
                    } else if (rb.position.x > worldCoords.worldRight + worldSpawnBuffer) {
                        Debug.Log("reached right bounds");
                        rb.velocity = new Vector2(-speed, 0);

                        // check if need to dodge
                    } else if (dodgeIsCharged && isThreatened()) {
                        Debug.Log("performing dodge");
                        performDodge();

                        // check if can attack
                    } else if (attackIsCharged) {
                        clearAllRoutines();
                        moveToAttackPosition();
                    }
                    break;
                }
            case Hammer.State.MOVING_TO_ATTACK: {
                    if (isInAttackPosition()) {
                        clearAllRoutines();
                        rb.position = new Vector2(targetX, rb.position.y);
                        rb.velocity = Vector2.zero;
                        state = Hammer.State.ATTACKING;
                        activeRoutines.Add(StartCoroutine(executeAttackSequence()));
                    }
                    break;
                }
            case Hammer.State.ATTACKING: {
                    break;
                }
        }
    }

    private bool isThreatened() {
        int hits = Physics2D.CircleCastNonAlloc(
            rb.position,
            dodgeCheckRadius,
            rb.velocity,
            dodgeCheckResults,
            dodgeCheckDistance,
            dodgeCheckLayerMask.value
        );
        return hits > 0;
    }

    private void performDodge() {
        rb.velocity = -rb.velocity;
        dodgeIsCharged = false;
        activeRoutines.Add(StartCoroutine(rechargeDodge()));
    }

    private void moveToAttackPosition() {
        state = Hammer.State.MOVING_TO_ATTACK;

        targetX = targetProvider().x;
        rb.velocity = targetX > rb.position.x
                ? new Vector2(speed, 0)
                : new Vector2(-speed, 0);
    }

    private bool isInAttackPosition() {
        return (rb.velocity.x < 0 && rb.position.x <= targetX)
            || (rb.velocity.x > 0 && rb.position.x >= targetX);
    }

    private void clearAllRoutines() {
        foreach (var routine in activeRoutines) {
            StopCoroutine(routine);
        }
        activeRoutines.Clear();
    }
}
