using UnityEngine;

public class ICBM : MonoBehaviour {
    public float worldSpawnBuffer = 1;
    public float layerZ = 5;
    public Explosion explosionPrefab;
    public TrailSettings trailSettings;

    private float thrust;
    private Vector3 thrustVector;
    private Rigidbody2D _rb;
    private StateUpdater stateUpdater;

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
        this.stateUpdater = stateUpdater;
        Vector3 spawnPosition = calculateSpawnPosition(worldCoords, targetCoords.x);
        float deviance = UnityEngine.Random.value * weaponData.accuracy.evaluate(stageProgress);
        if (UnityEngine.Random.value < 0.5f) {
            deviance = -deviance;
        }
        this.thrust = weaponData.primaryAcceleration.evaluate(stageProgress);
        Vector3 targetPosition = new Vector3(targetCoords.x + deviance, targetCoords.y, layerZ);
        this.thrustVector = (targetPosition - spawnPosition).normalized;
        transform.position = spawnPosition;

        gameObject.SetActive(true);
        rb.AddForce(thrustVector * weaponData.primaryImpulse.evaluate(stageProgress));

        var trail = ObjectPoolManager.Instance.getObjectInstance(trailSettings.prefab.gameObject).GetComponent<LinearTrail>();
        trail.initialise(gameObject, trailSettings);
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
        stateUpdater.onAttackDestroyed();
        gameObject.SetActive(false);

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(transform.position);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log($"OnCollisionEnter2D {gameObject.name} -> {other.gameObject.name} on layer {LayerMask.LayerToName(other.gameObject.layer)}");
        explode();
    }


    private void FixedUpdate() {
        rb.AddForce(thrustVector * thrust, ForceMode2D.Force);
    }
}
