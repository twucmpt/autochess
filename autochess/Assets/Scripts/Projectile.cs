using UnityEngine;

class Projectile : MonoBehaviour {
    public Vector3 direction;
    public float speed;
    public int damage;
    public Unit user;

    Rigidbody2D rb;
    void Start() {rb = GetComponent<Rigidbody2D>();}

    void Update() {rb.velocity = direction * speed;}

    void OnTriggerEnter2D (Collider2D collider) {
        Entity unit = collider.transform.root.GetComponent<Entity>();
        if (unit == user) return;

        unit.TakeDamage(damage);
        Destroy(gameObject);
    }
}