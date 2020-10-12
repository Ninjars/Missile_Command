using UnityEngine;

public class TargetMarker : MonoBehaviour {
    public float layerZ = 7;
    private Missile missile;

    public void configure(Missile missile, Vector2 position) {
        this.missile = missile;
        transform.position = new Vector3(position.x, position.y, layerZ);
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject == missile.gameObject) {
            missile.explode();
            gameObject.SetActive(false);
        }
    }
}
