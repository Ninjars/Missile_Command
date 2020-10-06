using System.Collections;
using System.Collections.Generic;
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
        Debug.Log($"onTriggerEnter2D {other.gameObject}");
        if (other.gameObject == missile.gameObject) {
            missile.explode();
            gameObject.SetActive(false);
        }
    }
}
