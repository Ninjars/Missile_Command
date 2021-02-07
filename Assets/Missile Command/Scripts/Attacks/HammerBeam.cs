using Shapes;
using UnityEngine;

public class HammerBeam : MonoBehaviour {
    public Explosion interruptExplosionPrefab;
    public Explosion impactExplosionPrefab;
    public LayerMask beamKillLayerMask;
    public Rectangle beam;
    public float maxBeamWidth;
    public float killBeamWidth;
    public int maxInterruptExplosions = 20;

    private float startTime;
    private float attackTime;
    private float groundOffset;
    private Colors colors { get { return Colors.Instance; } }

    internal void beginSequence(Vector2 position, float groundLevel, float attackTime) {
        this.groundOffset = groundLevel - position.y;
        this.attackTime = attackTime;
        transform.position = position;
        startTime = Time.time;
        beam.Color = colors.attackColor;
        positionBeam();
        updateBeamVisuals(0);
    }

    private void positionBeam() {
        beam.transform.localPosition = new Vector3(
            0,
            groundOffset / 2f,
            transform.position.z
        );
        beam.Height = groundOffset;
    }

    internal void onInterrupt() {
        float maxY = groundOffset * 0.7f;
        int explosionCount = Mathf.RoundToInt(((Time.time - startTime) / attackTime) * maxInterruptExplosions);
        for (int i = 0; i < explosionCount; i++) {
            var randomX = UnityEngine.Random.value;
            var x = (randomX * randomX) * maxBeamWidth * 2 - maxBeamWidth;
            if (UnityEngine.Random.value < 0.5f) {
                x = -x;
            }
            x = transform.position.x + x;
            var randomY = UnityEngine.Random.value;
            var y = transform.position.y + (randomY * randomY) * maxY;
            var explosion = ObjectPoolManager.Instance.getObjectInstance(interruptExplosionPrefab.gameObject).GetComponent<Explosion>();
            explosion.boom(
                new Vector2(x, y),
                colors.attackExplodeColor,
                0.05f + UnityEngine.Random.value * 0.2f,
                0.2f + UnityEngine.Random.value * 0.6f
            );
        }
        GameObject.Destroy(gameObject);
    }

    private void Update() {
        float progress = (Time.time - startTime) / attackTime;
        updateBeamVisuals(progress);
        if (progress >= 1) {
            fire();
        }
    }

    private void updateBeamVisuals(float progress) {
        beam.Width = Mathf.Lerp(maxBeamWidth, killBeamWidth, progress);
        beam.Color = new Color(beam.Color.r, beam.Color.g, beam.Color.b, progress);
    }

    private void fire() {
        var explosion = GameObject.Instantiate<Explosion>(impactExplosionPrefab);
        explosion.boom(transform.position + groundOffset * Vector3.up, colors.attackExplodeColor);

        Collider2D[] hits = Physics2D.OverlapAreaAll(
            transform.position - killBeamWidth * Vector3.right, 
            transform.position + groundOffset * Vector3.up + killBeamWidth * Vector3.right,
            beamKillLayerMask
        );
        foreach (var hit in hits) {
            var explodable = hit.transform.gameObject.GetComponent<Explodable>();
            if (hit.transform.gameObject.GetComponent<Hammer>() == null && explodable != null) {
                explodable.explode();
            }
        }

        GameObject.Destroy(gameObject);
    }
}
