using Shapes;
using UnityEngine;

public class Missile : MonoBehaviour {
    public float missileLayerZ = 8;
    public float launchImpulse = 20;
    public float thrust = 10;
    public Explosion explosionPrefab;
    public GameObject targetMarker;
    public TrailSettings trailSettings;
    private TargetMarker marker;
    private Vector2 target;
    private Vector2 facing;
    private Colors colors { get { return Colors.Instance; }}

    private Rigidbody2D _rb;
    private LinearTrail trail;

    private Rigidbody2D rb {
        get {
            if (_rb == null) {
                _rb = GetComponent<Rigidbody2D>();
            }
            return _rb;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        explode();
    }

    public void explode() {
        if (!gameObject.activeSelf) return;

        gameObject.SetActive(false);

        var explosion = ObjectPoolManager.Instance.getObjectInstance(explosionPrefab.gameObject).GetComponent<Explosion>();
        explosion.boom(rb.position, colors.missileExplodeColor);
        
        if (marker != null) {
            marker.gameObject.SetActive(false);
            marker = null;
        }
    }

    public void launch(Vector2 position, Vector2 target) {
        this.target = target;
        this.facing = (target - position).normalized;
        transform.position = new Vector3(position.x, position.y, missileLayerZ);

        GetComponentInChildren<Polyline>().Color = colors.missileColor;

        this.marker = ObjectPoolManager.Instance.getObjectInstance(targetMarker.gameObject).GetComponent<TargetMarker>();
        marker.configure(this, target);

        gameObject.SetActive(true);
        rb.AddForce(facing * launchImpulse, ForceMode2D.Impulse);
        
        trail = ObjectPoolManager.Instance.getObjectInstance(trailSettings.prefab.gameObject).GetComponent<LinearTrail>();
        trail.initialise(gameObject, trailSettings, colors.missileTrailColor);
    }

    private void FixedUpdate() {
        rb.AddForce(facing * thrust, ForceMode2D.Force);
    }

    private void OnDrawGizmos() {
        Debug.DrawLine(
            new Vector3(rb.position.x, rb.position.y, missileLayerZ),
            new Vector3(target.x, target.y, missileLayerZ),
            Color.green
        );
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
