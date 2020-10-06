using UnityEngine;

public class ICBM : MonoBehaviour {
    public float worldSpawnBuffer = 1;
    public float layerZ = 5;
    public float detonationRange = 0.5f;
    public MissileExplosion explosionPrefab;

    private Vector3 targetPosition;
    private float thrust;
    private Vector3 thrustVector;
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
        ICBMData weaponData,
        float stageProgress,
        Vector2 targetCoords
    ) {
        Vector3 spawnPosition = calculateSpawnPosition(worldCoords, targetCoords.x);
        float deviance = UnityEngine.Random.value * weaponData.accuracy.evaluate(stageProgress);
        if (UnityEngine.Random.value < 0.5f) {
            deviance = -deviance;
        }
        this.targetPosition = new Vector3(targetCoords.x + deviance, targetCoords.y, layerZ);
        this.thrust = weaponData.primaryAcceleration.evaluate(stageProgress);
        this.thrustVector = (targetPosition - spawnPosition).normalized;
        transform.position = spawnPosition;

        gameObject.SetActive(true);
        rb.AddForce(thrustVector * weaponData.primaryImpulse.evaluate(stageProgress));
    }

    private Vector3 calculateSpawnPosition(WorldCoords worldCoords, float targetX) {
        float x;
        Debug.Log($"calculateSpawnPosition() targetX {targetX} worldCenter {worldCoords.centerX}");
        if (targetX > worldCoords.centerX) {
            x = worldCoords.worldLeft - worldSpawnBuffer;
        } else {
            x = worldCoords.worldRight + worldSpawnBuffer;
        }
        return new Vector3(x * UnityEngine.Random.value, worldCoords.worldTop+worldSpawnBuffer, layerZ);
    }

    private void explode() {
        gameObject.SetActive(false);

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<MissileExplosion>();
        explosion.boom(transform.position);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log($"OnCollisionEnter2D {gameObject.name} -> {other.gameObject.name} on layer {LayerMask.LayerToName(other.gameObject.layer)}");
        explode();
    }


    private void FixedUpdate() {
        if (Vector3.Distance(transform.position, targetPosition) < detonationRange) {
            explode();
        } else {
            rb.AddForce(thrustVector * thrust, ForceMode2D.Force);
        }
    }

    private void OnDrawGizmos() {
        Debug.DrawLine(
            transform.position,
            targetPosition,
            Color.red
        );
    }
}
