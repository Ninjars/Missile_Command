using UnityEngine;

public class FaceDirectionOfTravel : MonoBehaviour {
    private Rigidbody2D rb;
    
    private void Awake() {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    void Update() {
        transform.rotation = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.up, rb.velocity, Vector3.back), Vector3.back);
    }
}
