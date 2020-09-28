using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {
    public float thrust = 10;
    private Rigidbody2D rb;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void launch(Vector3 position, Vector2 target) {
        rb.position = position;
        var vector = (target - rb.position).normalized;
        rb.AddForce(vector * thrust, ForceMode2D.Impulse);
        gameObject.SetActive(true);
    }

    private void OnDisable() {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
    }
}
