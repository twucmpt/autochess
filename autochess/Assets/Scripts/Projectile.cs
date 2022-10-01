using UnityEngine;

public class Projectile : MonoBehaviour {
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public ProjectileAttack ability;

    Rigidbody2D rb;
    void Start() {rb = GetComponent<Rigidbody2D>();}

    void FixedUpdate() {rb.velocity = direction * ability.speed;}

    void OnTriggerEnter2D (Collider2D collider) {
        ability.OnProjectileHit(this, collider);
    }
}