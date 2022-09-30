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
        Unit unit = collider.transform.root.GetComponent<Unit>();
        if (unit == user) return;

        unit.TakeDamage(damage);
        Destroy(gameObject);
    }
}