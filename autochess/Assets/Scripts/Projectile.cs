using UnityEngine;

public class Projectile : MonoBehaviour {
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public ProjectileAttack ability;

    Rigidbody rb;
    void Start() {rb = GetComponent<Rigidbody>();}

    void FixedUpdate() {rb.velocity = direction * ability.speed;}

    void OnTriggerEnter (Collider collider) {
        ability.OnProjectileHit(this, collider);
    }

    void Update() {
        if (GameManager.Instance.currentPhase != GamePhase.Combat) Destroy(gameObject);
    }
}